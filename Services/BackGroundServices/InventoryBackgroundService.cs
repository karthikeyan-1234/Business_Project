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
        private readonly IServiceProvider _serviceProvider;
        private string? exchange;
        private string? queue;
        private string? routingKey;
        private string? hostName;
        private int port;
        public InventoryBackgroundService(IConfiguration configuration, IServiceProvider _serviceProvider)
        {
            this._serviceProvider = _serviceProvider;
            exchange = configuration.GetSection("RabbitMQ").GetSection("Purchase").GetSection("Exchange").Value;
            queue = configuration.GetSection("RabbitMQ").GetSection("Purchase").GetSection("Queue").Value;
            routingKey = configuration.GetSection("RabbitMQ").GetSection("Purchase").GetSection("RoutingKey").Value;
            hostName = configuration.GetSection("RabbitMQ").GetSection("Purchase").GetSection("HostName").Value;
            port = Convert.ToInt16(configuration.GetSection("RabbitMQ").GetSection("Purchase").GetSection("Port").Value);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Inventory Service Background Service...");

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
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }

        }
    }
}
