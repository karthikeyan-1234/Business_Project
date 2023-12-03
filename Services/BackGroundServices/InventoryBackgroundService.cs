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
    public class QAndRoutingKey
    {
        public string Queue { get; set; }
        public string Key { get; set; }
        public int id { get; set; }
    }

    public class InventoryBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration configuration;
        private string? exchange;
        private string? queue;
        private string? routingKey;
        private string? hostName;
        private int port;
        List<QAndRoutingKey> QAndRoutingKeys;

        public InventoryBackgroundService(IConfiguration configuration, IServiceProvider _serviceProvider)
        {
            this._serviceProvider = _serviceProvider;
            this.configuration = configuration;
            exchange = configuration.GetSection("RabbitMQ").GetSection("NewPurchaseDetail").GetSection("Exchange").Value;
            queue = configuration.GetSection("RabbitMQ").GetSection("NewPurchaseDetail").GetSection("Queue").Value;
            routingKey = configuration.GetSection("RabbitMQ").GetSection("NewPurchaseDetail").GetSection("RoutingKey").Value;
            hostName = configuration.GetSection("RabbitMQ").GetSection("NewPurchaseDetail").GetSection("HostName").Value;
            port = Convert.ToInt16(configuration.GetSection("RabbitMQ").GetSection("NewPurchaseDetail").GetSection("Port").Value);
            var section = configuration.GetSection("RabbitMQ").GetChildren();
            QAndRoutingKeys = configuration.GetSection("RoutingKeys").Get<List<QAndRoutingKey>>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Inventory Service Background Service...");

            var factory = new ConnectionFactory() { HostName = hostName, Port = port };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);

                foreach (QAndRoutingKey obj in QAndRoutingKeys)
                {
                    channel.QueueDeclare(queue: obj.Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: obj.Queue, exchange: exchange, routingKey: obj.Key);
                }

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    if (ea.RoutingKey == configuration.GetSection("RabbitMQ").GetSection("NewPurchaseDetail").GetSection("RoutingKey").Value)
                    {
                        using (IServiceScope scope = _serviceProvider.CreateScope())
                        {
                            IInventoryService scopedProcessingService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                            var _PurchaseDetail = JsonSerializer.Deserialize<PurchaseDetail>(message);
                            var newInventoryRequest = new NewInventoryRequest() { itemId = _PurchaseDetail.itemId, qty = _PurchaseDetail.qty, lastUpdated = DateTime.Now, Notes = "Purchase added notification via RabbitMQ" };
                            var users = await scopedProcessingService.AddInventoryAsync(newInventoryRequest);   //Use the without cache option as the httpContextSession, can't be accessed from kgroundService                        }

                        }
                    }
                };

            foreach (QAndRoutingKey obj in QAndRoutingKeys)
            {
                channel.BasicConsume(queue: obj.Queue, autoAck: true, consumer: consumer);
            }
                
            await Task.Delay(Timeout.Infinite, stoppingToken);
            }

        }

        void oldCode()
        {
            var factory = new ConnectionFactory() { HostName = hostName, Port = port };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
                var result = channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);
                var queue_name = result.QueueName;
                channel.QueueBind(queue: queue_name, exchange: exchange, routingKey: routingKey);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var _PurchaseDetail = JsonSerializer.Deserialize<PurchaseDetail>(message);


                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        IInventoryService scopedProcessingService =
                            scope.ServiceProvider.GetRequiredService<IInventoryService>();

                        var newInventoryRequest = new NewInventoryRequest() { itemId = _PurchaseDetail.itemId, qty = _PurchaseDetail.qty, lastUpdated = DateTime.Now, Notes = "Purchase added notification via RabbitMQ" };
                        var users = await scopedProcessingService.AddInventoryAsync(newInventoryRequest);   //Use the without cache option as the httpContextSession, can't be accessed from BackgroundService
                    }


                    System.Console.WriteLine($"Received message in Inventory Service {message}");

                };

                channel.BasicConsume(queue: queue_name, autoAck: true, consumer: consumer);
            }
        }
    }
}
