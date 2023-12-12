using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.BackGroundServices
{


    public class InventoryBackgroundService : BackgroundService
    {
        private string? hostName;
        private int port;
        private string? exchange;

        private string? inventory_queue;
        private string? inventory_req_routingKey;
        private string? inventory_res_routingKey;
        private string? inventory_upd_routingKey;

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private readonly IServiceProvider _serviceProvider;

        public InventoryBackgroundService(IConfiguration configuration, IServiceProvider _serviceProvider)
        {
            hostName = configuration.GetSection("RabbitMQ").GetSection("HostName").Value;
            port = Convert.ToInt16(configuration.GetSection("RabbitMQ").GetSection("Port").Value);


            exchange = configuration.GetSection("RabbitMQ").GetSection("Exchange").Value;

            inventory_queue = configuration.GetSection("InventoryQueue").GetSection("Queue").Value;
            inventory_req_routingKey = configuration.GetSection("InventoryQueue").GetSection("Request_Key").Value;
            inventory_res_routingKey = configuration.GetSection("InventoryQueue").GetSection("Response_Key").Value;
            inventory_upd_routingKey = configuration.GetSection("InventoryQueue").GetSection("Update_Key").Value;

            factory = new ConnectionFactory() { HostName = hostName, Port = port };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            this._serviceProvider = _serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
            channel.QueueDeclare(queue: inventory_queue, durable: true, exclusive: false, autoDelete: true);

            channel.QueueBind(queue: inventory_queue, exchange: exchange, routingKey: inventory_req_routingKey);
            channel.QueueBind(queue: inventory_queue, exchange: exchange, routingKey: inventory_upd_routingKey);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var guid = ea.BasicProperties.CorrelationId;

                if (ea.BasicProperties.CorrelationId is not null)
                {
                    #region Handle inventory requests
                    if (ea.RoutingKey == inventory_req_routingKey)
                    {

                        Console.WriteLine($"2. Received purchase details from Purchase service...{message} @ " + DateTime.Now);
                        Console.WriteLine(guid);

                        SendInventoryResponse(guid, message);
                    }
                    #endregion

                    #region Handle inventory updates
                    if (ea.RoutingKey == inventory_upd_routingKey)
                    {
                        Console.WriteLine($"2. Updating inventory with purchase details from Purchase service...{message} @ " + DateTime.Now);
                        Console.WriteLine(guid);

                        await UpdateInventory(guid, message);
                    }
                    #endregion
                }
            };

            channel.BasicConsume(queue: inventory_queue, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);

        }


        void SendInventoryResponse(string guid, string message)
        {
            channel.ExchangeDeclare(exchange: "ERP", type: ExchangeType.Topic);
            channel.QueueDeclare(queue: "Inventory_Responses", durable: true, exclusive: false, autoDelete: true);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            properties.CorrelationId = guid;

            var inventory_status = Encoding.UTF8.GetBytes("Inventory is 1000" + " [" + message + "]");

            Console.WriteLine("3. Sending inventory response to Purchase service... " + DateTime.Now);
            Console.WriteLine(guid);

            channel.BasicPublish(exchange: "ERP", routingKey: inventory_res_routingKey, basicProperties: properties, body: inventory_status);
        }

        async Task UpdateInventory(string guid, string message)
        {
            var purchDetail = JsonSerializer.Deserialize<PurchaseDetail>(message);

            using (var scope = _serviceProvider.CreateScope())
            {
                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                await inventoryService.UpsertInventoryAsync(new CommonLibrary.DTOs.InventoryDTO { itemId = purchDetail.itemId, qty = purchDetail.qty, lastUpdated = DateTime.Now, Notes = "Updated by Purchase service" });
            }
        }
    }
}
