using MediatR;

using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.Models;
using Services.CQRS.Queries.Inventory_Queries;
using Services.CQRS.Queries.Inventory;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Services.CQRS.Handlers.Inventory.Broker
{
    public class GetItemInventoryQueryBrokerHandler : IRequestHandler<Services.CQRS.Queries.Inventory.GetItemInventoryQueryBroker, CommonLibrary.Models.Inventory>
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        private string? hostName;
        private int port;
        private string? exchange;

        private string? inventory_queue;
        private string? inventory_req_routingKey;
        private string? inventory_res_routingKey;
        private string? inventory_upd_routingKey;

        private string? guid;

        public GetItemInventoryQueryBrokerHandler(IConfiguration configuration)
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

        public async Task<CommonLibrary.Models.Inventory> Handle(GetItemInventoryQueryBroker request, CancellationToken cancellationToken)
        {
            SendInventoryRequest(Int16.Parse(request.item_id));
            var result = await GetInventory();

            CommonLibrary.Models.Inventory ?obj = default;

            try
            {
                if(result is not null)
                    obj = JsonSerializer.Deserialize<CommonLibrary.Models.Inventory>(result);
            }
            catch { }

            return  obj;
        }
    }
}
