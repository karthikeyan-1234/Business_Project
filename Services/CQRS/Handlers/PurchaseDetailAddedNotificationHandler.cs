using MediatR;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Services.CQRS.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers
{
    public class PurchaseDetailAddedNotificationHandler : INotificationHandler<PurchaseDetailAddedNotification>
    {
        private string? exchange;
        private string? queue;
        private string? routingKey;
        private string? hostName;
        private int port;

        public PurchaseDetailAddedNotificationHandler(IConfiguration configuration)
        {
            exchange = configuration.GetSection("RabbitMQ").GetSection("PurchaseDetail").GetSection("Exchange").Value;
            queue = configuration.GetSection("RabbitMQ").GetSection("PurchaseDetail").GetSection("Queue").Value;
            routingKey = configuration.GetSection("RabbitMQ").GetSection("PurchaseDetail").GetSection("RoutingKey").Value;
            hostName = configuration.GetSection("RabbitMQ").GetSection("PurchaseDetail").GetSection("HostName").Value;
            port = Convert.ToInt16(configuration.GetSection("RabbitMQ").GetSection("PurchaseDetail").GetSection("Port").Value);
        }
        public Task Handle(PurchaseDetailAddedNotification notification, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { HostName = hostName, Port = port };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
                var result = channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);
                var queue_name = result.QueueName;
                channel.QueueBind(queue: queue_name, exchange: exchange, routingKey: routingKey);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                var body1 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(notification.newPurchaseDetail));

                channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: properties, body: body1);
            }

            return Task.CompletedTask;
        }
    }
}
