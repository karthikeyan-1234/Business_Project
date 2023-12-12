using MediatR;

using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;

using Services.CQRS.Notifications.Inventory_Notifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers.Inventory_Handlers
{
    public class UpdateInventoryNotificationHandler : INotificationHandler<UpdateInventoryNotification>
    {
        private string? hostName;
        private int port;
        private string? exchange;

        private string? inventory_queue;
        private string? inventory_upd_routingKey;

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        private string? guid;

        public UpdateInventoryNotificationHandler(IConfiguration configuration)
        {
            hostName = configuration.GetSection("RabbitMQ").GetSection("HostName").Value;
            port = Convert.ToInt16(configuration.GetSection("RabbitMQ").GetSection("Port").Value);

            exchange = configuration.GetSection("RabbitMQ").GetSection("Exchange").Value;
            factory = new ConnectionFactory() { HostName = hostName, Port = port };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            inventory_queue = configuration.GetSection("InventoryQueue").GetSection("Queue").Value;
            inventory_upd_routingKey = configuration.GetSection("InventoryQueue").GetSection("Update_Key").Value;

            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
            channel.QueueDeclare(queue: inventory_queue, durable: true, exclusive: false, autoDelete: true);
        }
        public Task Handle(UpdateInventoryNotification notification, CancellationToken cancellationToken)
        {
            guid = Guid.NewGuid().ToString();

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            properties.CorrelationId = guid;

            var inventory_status = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(notification.revisedPurchaseDetail));

            Console.WriteLine("1. Sending update inventory rquest to Inventory service... " + DateTime.Now);
            Console.WriteLine(guid);

            channel.BasicPublish(exchange: exchange, routingKey: inventory_upd_routingKey, basicProperties: properties, body: inventory_status);

            return Task.CompletedTask;
        }
    }
}
