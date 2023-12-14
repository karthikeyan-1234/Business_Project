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

        string? final_result = null;

        public GetItemInventoryQueryBrokerHandler(IConfiguration configuration)
        {
            hostName = configuration.GetSection("RabbitMQ").GetSection("HostName").Value;
            port = Convert.ToInt16(configuration.GetSection("RabbitMQ").GetSection("Port").Value);


            exchange = configuration.GetSection("RabbitMQ").GetSection("Exchange").Value;

            inventory_queue = configuration.GetSection("InventoryQueue").GetSection("Queue").Value;
            inventory_req_routingKey = configuration.GetSection("InventoryQueue").GetSection("Request_Key").Value;
            inventory_res_routingKey = configuration.GetSection("InventoryQueue").GetSection("Response_Key").Value;
            inventory_upd_routingKey = configuration.GetSection("InventoryQueue").GetSection("Update_Key").Value;           
            
        }

        string SendInventoryRequest(int item_id)
        {
            var guid = Guid.NewGuid().ToString();

            var factory = new ConnectionFactory() { HostName = hostName, Port = port };
            using (var connection = factory.CreateConnection())
            {
                channel = connection.CreateModel();

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;
                properties.CorrelationId = guid;

                var inventory_status = Encoding.UTF8.GetBytes(item_id.ToString());

                Console.WriteLine("1. Sending inventory rquest to Inventory service... " + DateTime.Now);
                Console.WriteLine(guid);

                channel.BasicPublish(exchange: exchange, routingKey: inventory_req_routingKey, basicProperties: properties, body: inventory_status);
            }
                return guid;
        }

        async Task<string> GetInventory(string s_guid)
        {

            #region Wait for response from Inventory API with Inventory status

            Console.WriteLine("4. Setting up consumer to detect incoming inventory response... " + DateTime.Now);
            Console.WriteLine(s_guid);

            var factory = new ConnectionFactory() { HostName = hostName, Port = port };
            using (var connection = factory.CreateConnection())
            {
                channel = connection.CreateModel();

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    if (ea.BasicProperties.CorrelationId == s_guid)
                    {
                        if (ea.RoutingKey == inventory_res_routingKey)
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine($"5. Inventory status obtained.. {s_guid}!!");
                            Console.WriteLine(message);
                            final_result = message;
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                };

                channel.BasicConsume(queue: "Inventory_Responses", autoAck: false, consumer: consumer);
                await Task.Delay(5000);
            }
            // Wait for completion or timeout

            return final_result;

            #endregion
        }

        public async Task<CommonLibrary.Models.Inventory> Handle(GetItemInventoryQueryBroker request, CancellationToken cancellationToken)
        {
            var kguid = SendInventoryRequest(Int16.Parse(request.item_id));
            var result = await GetInventory(kguid);

            CommonLibrary.Models.Inventory ?obj = default;

            try
            {
                if(result is not null && result.Length > 0)
                    obj = JsonSerializer.Deserialize<CommonLibrary.Models.Inventory>(result);
            }
            catch { }

            return  obj;
        }
    }
}
