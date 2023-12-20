using AutoMapper;
using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using CommonLibrary.Repositories;
using CommonLibrary.Utilities;

using MediatR;

using Microsoft.Extensions.Configuration;


using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Services.CQRS.Commands;
using Services.CQRS.Commands.Purchase_Commands;
using Services.CQRS.Notifications;
using Services.CQRS.Notifications.Inventory_Notifications;
using Services.CQRS.Queries.Inventory;
using Services.CQRS.Queries.Purchases;

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

        PurchaseDetail GetPurchaseDetailEntry(int id)
        {
            return purchaseDetailRepo.Find(p => p.id == id).FirstOrDefault();
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
            var result = await mediator.Send(new GetItemInventoryQueryBroker(item_id.ToString()));
            if (result is not null)
                return Utility.JsonObjectSerializer(result);
            else
                return default;
        }

        public async Task<PurchaseDetailDTO> UpdatePurchaseDetailAsync(UpdatePurchaseDetailRequest request)
        {
            InventoryDTO ?current_inventory = null;


            #region Purchase details update
            //Get current purchase details

            var oldDetail = GetPurchaseDetailEntry(request.id);
            float newQty = 0;

            //Now update the purchase details

            var newDetail = mapper.Map<PurchaseDetail>(request);
            var result = await mediator.Send(new UpdatePurchaseDetailCommand(newDetail));
            #endregion


            //Get current inventory status for the item

            var inventory_str = await GetInventoryStatus(request.itemId); // Handle null JSON Deserialization here

            if (inventory_str is not null)
            {
                current_inventory = JsonSerializer.Deserialize<InventoryDTO>(inventory_str);
                Console.WriteLine("Current Inventory...");
                Console.Write(inventory_str);
            }
            else
            {
                Console.WriteLine($"Unable to get current inventory for item : {request.itemId}, from Inventory service");
            }

            #region Inventory Update
            /* Disable Inventory Update for the time being
            //If new value of purchase detail is lower than old value, then reduce the difference in Inventory for the item

            if (newDetail.qty < oldDetail.qty && current_inventory is not null)
                newQty = (oldDetail.qty - newDetail.qty) * -1; //Send negative value to Inventory to reduce

            //If new value of purchase detail is higher than old value, then increase the difference in Inventory for the item

            else if (newDetail.qty > oldDetail.qty && current_inventory is not null)
                newQty = newDetail.qty - oldDetail.qty; //Send positive value to Inventory to increase

            else if(current_inventory is not null)
                newQty = current_inventory.qty;

            
            result.qty = newQty;     

            var msg = new UpdateInventoryNotification(result);
            await mediator.Publish(msg);

            */
            #endregion

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

        public async Task<IEnumerable<PurchaseDetailDTO>> GetPurchaseDetailsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var qry = new GetPurchaseDetailsByDateQuery(startDate, endDate);
            var result = await mediator.Send(qry);
            if(result is not null)
                return mapper.Map<IEnumerable<PurchaseDetailDTO>>(result);

            return default;
        }
    }
}