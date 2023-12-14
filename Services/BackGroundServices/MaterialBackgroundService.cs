using CommonLibrary.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace Services.BackGroundServices
{
    public class MaterialBackgroundService : BackgroundService
    {
        private string? hostName;
        private int port;
        private string? exchange;

        private string? material_queue;
        private string? material_req_routingKey;
        private string? material_res_routingKey;
        private string? material_upd_routingKey;

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private readonly IServiceProvider _serviceProvider;

        public MaterialBackgroundService(IConfiguration configuration, IServiceProvider _serviceProvider)
        {
            hostName = configuration.GetSection("RabbitMQ").GetSection("HostName").Value;
            port = Convert.ToInt16(configuration.GetSection("RabbitMQ").GetSection("Port").Value);


            exchange = configuration.GetSection("RabbitMQ").GetSection("Exchange").Value;

            material_queue = configuration.GetSection("MaterialQueue").GetSection("Queue").Value;
            material_req_routingKey = configuration.GetSection("MaterialQueue").GetSection("Request_Key").Value;
            material_res_routingKey = configuration.GetSection("MaterialQueue").GetSection("Response_Key").Value;
            material_upd_routingKey = configuration.GetSection("MaterialQueue").GetSection("Update_Key").Value;

            factory = new ConnectionFactory() { HostName = hostName, Port = port };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            this._serviceProvider = _serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
            channel.QueueDeclare(queue: material_queue, durable: true, exclusive: false, autoDelete: true);

            channel.QueueBind(queue: material_queue, exchange: exchange, routingKey: material_req_routingKey);
            channel.QueueBind(queue: material_queue, exchange: exchange, routingKey: material_upd_routingKey);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var guid = ea.BasicProperties.CorrelationId;

                if (ea.BasicProperties.CorrelationId is not null)
                {
                    #region Handle material requests
                    if (ea.RoutingKey == material_req_routingKey)
                    {

                        Console.WriteLine($"2. Received get all materials from Purchase service...{message} @ " + DateTime.Now);
                        Console.WriteLine(guid);

                        await SendMaterialResponse(guid, message);
                    }
                    #endregion
                }
            };

            channel.BasicConsume(queue: material_queue, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);

        }

        async Task SendMaterialResponse(string guid, string message)
        {
            object? result = null;

            using (var scope = _serviceProvider.CreateScope())
            {
                var Service = scope.ServiceProvider.GetRequiredService<IMaterialService>();
                result = await Service.GetAllMaterialsAsync();
            }

            channel.ExchangeDeclare(exchange: "ERP", type: ExchangeType.Topic);
            channel.QueueDeclare(queue: "Material_Responses", durable: true, exclusive: false, autoDelete: true);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            properties.CorrelationId = guid;

            var inventory_status = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result));

            Console.WriteLine("3. Sending material response to Purchase service... " + DateTime.Now);
            Console.WriteLine(guid);

            channel.BasicPublish(exchange: "ERP", routingKey: material_res_routingKey, basicProperties: properties, body: inventory_status);
        }
    }
}
