using AutoMapper;
using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using CommonLibrary.Repositories;
using MediatR;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Services.CQRS.Commands;
using Services.CQRS.Commands.Purchase_Commands;
using Services.CQRS.Notifications;
using Services.CQRS.Notifications.Inventory_Notifications;

using System.Text;
using System.Text.Json;

namespace Services
{
    public class PurchaseService : IPurchaseService
    {
        IGenericRepository<Purchase, PurchaseDBContext> purchaseRepo;
        IGenericRepository<PurchaseDetail,PurchaseDBContext> purchaseDetailRepo;
        IMapper mapper;
        ICacheManager cache;
        IMediator mediator;
        IConfiguration configuration;

        private string? hostName;
        private int port;
        private string? exchange;

        private string? inventory_queue;
        private string? inventory_req_routingKey;
        private string? inventory_res_routingKey;

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        private string? guid;

        public PurchaseService(IGenericRepository<Purchase, PurchaseDBContext> purchaseRepo, IGenericRepository<PurchaseDetail, PurchaseDBContext> purchaseDetailRepo, 
            IMapper mapper,ICacheManager cache,IMediator mediator, IConfiguration configuration)
        {
            this.purchaseRepo = purchaseRepo;
            this.purchaseDetailRepo = purchaseDetailRepo;
            this.mapper = mapper;
            this.cache = cache;
            this.mediator = mediator;
            this.configuration = configuration;

            hostName = configuration.GetSection("RabbitMQ").GetSection("HostName").Value;
            port = Convert.ToInt16(configuration.GetSection("RabbitMQ").GetSection("Port").Value);

            exchange = configuration.GetSection("RabbitMQ").GetSection("Exchange").Value;

            inventory_queue = configuration.GetSection("InventoryQueue").GetSection("Queue").Value;
            inventory_req_routingKey = configuration.GetSection("InventoryQueue").GetSection("Request_Key").Value;
            inventory_res_routingKey = configuration.GetSection("InventoryQueue").GetSection("Response_Key").Value;

            factory = new ConnectionFactory() { HostName = hostName, Port = port };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
            channel.QueueDeclare(queue: inventory_queue, durable: true, exclusive: false, autoDelete: true);

            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
            channel.QueueDeclare(queue: "Inventory_Responses", durable: true, exclusive: false, autoDelete: true);
            channel.QueueBind(queue: "Inventory_Responses", exchange: exchange, routingKey: inventory_res_routingKey);

        }

        public async Task<PurchaseDTO> AddPurchaseAsync(NewPurchaseRequest request)
        {
            var newPurchase = mapper.Map<Purchase>(request);
            var result = await mediator.Send(new AddPurchaseCommand(newPurchase));
            return mapper.Map<PurchaseDTO>(result);
        }

        public async Task DeletePurchaseAsync(PurchaseDTO updatePurchase)
        {
            var purchase = mapper.Map<Purchase>(updatePurchase);
            purchaseRepo.Delete(purchase);
            await purchaseRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<PurchaseResultDTO>> GetAllPurchasesAsync()
        {
            var purchases = await cache.TryGetAsync<IEnumerable<Purchase>>("GetAllPurchases");
            if (purchases is null)
            {
                purchases = await purchaseRepo.GetAllAsync();
                await cache.TrySetAsync(purchases, "GetAllPurchases");
            }
            return mapper.Map<IEnumerable<PurchaseResultDTO>>(purchases);
        }

        public async Task<PurchaseDTO> UpdatePurchaseAsync(PurchaseDTO updatePurchase)
        {
            var purchase = purchaseRepo.Update(mapper.Map<Purchase>(updatePurchase));
            await purchaseRepo.SaveChangesAsync();
            return mapper.Map<PurchaseDTO>(purchase);

        }

        private async Task<Purchase> GetPurchase(int id)
        {
            return await purchaseRepo.GetById(id);
        }

        public async Task<IEnumerable<PurchaseDetailDTO>> GetPurchaseDetails(int purchaseId)
        {
            var purchase = await purchaseRepo.GetById(purchaseId);
            return mapper.Map<IEnumerable<PurchaseDetailDTO>>(purchase.PurchaseDetails);
        }

        public async Task<PurchaseDetailDTO> AddPurchaseDetailAsync(NewPurchaseDetailRequest request)
        {
            var newPurchaseDetail = mapper.Map<PurchaseDetail>(request);

            var result = await mediator.Send(new AddPurchaseDetailCommand(newPurchaseDetail));
            var msg = new UpdateInventoryNotification(result);
            await mediator.Publish(msg);
            return mapper.Map<PurchaseDetailDTO>(result);
        }

        public async Task<string> GetInventoryStatus(int item_id)
        {
            SendInventoryRequest(item_id);
            return await GetInventory();
        }

        void SendInventoryRequest(int item_id)
        {
            guid = Guid.NewGuid().ToString();

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            properties.CorrelationId = guid;

            var inventory_status = Encoding.UTF8.GetBytes(item_id.ToString());

            Console.WriteLine("1. Sending inventory rquest to Inventory service... " + DateTime.Now);
            Console.WriteLine(guid);

            channel.BasicPublish(exchange: exchange, routingKey: inventory_req_routingKey, basicProperties: properties, body: inventory_status);
        }

        async Task<string> GetInventory()
        {
            string final_result = String.Empty;

            #region Wait for response from Inventory API with Inventory status

            Console.WriteLine("4. Setting up consumer to detect incoming inventory response... " + DateTime.Now);
            Console.WriteLine($"5. Finding messages from exchange {exchange} with queue Inventory_Responses with routing key {inventory_res_routingKey}");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == guid)
                {
                    if (ea.RoutingKey == inventory_res_routingKey)
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"6. Inventory status obtained.. {guid}!!");
                        Console.WriteLine(message);
                        final_result = message;
                    }
                }
            };

            channel.BasicConsume(queue: "Inventory_Responses", autoAck: false, consumer: consumer);

            await Task.Delay(5000);

            return final_result;
            #endregion
        }

        public async Task<PurchaseDetailDTO> UpdatePurchaseDetailAsync(UpdatePurchaseDetailRequest request)
        {
            var newPurchaseDetail = mapper.Map<PurchaseDetail>(request);
            var result = await mediator.Send(new UpdatePurchaseDetailCommand(newPurchaseDetail));
            var msg = new UpdateInventoryNotification(result);
            await mediator.Publish(msg);
            return mapper.Map<PurchaseDetailDTO>(result);
        }

        public async Task<PurchaseDetailDTO> DeletePurchaseDetailAsync(UpdatePurchaseDetailRequest request)
        {
            var newPurchaseDetail = mapper.Map<PurchaseDetail>(request);
            var result = await mediator.Send(new DeletePurchaseDetailCommand(newPurchaseDetail));
            if(result.qty > 0) result.qty = result.qty * -1;
            var msg = new UpdateInventoryNotification(result);
            await mediator.Publish(msg);
            return mapper.Map<PurchaseDetailDTO>(result);
        }
    }
}