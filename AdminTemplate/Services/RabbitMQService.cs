using AdminTemplate.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public class RabbitMQService : IMessageQueueService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQService> _logger;
        private const string QUEUE_NAME = "supplier_queue";

        public RabbitMQService(
            IConfiguration configuration,
            ILogger<RabbitMQService> logger)
        {
            _logger = logger;

            try
            {
                var cloudAmqpUrl = configuration["CloudAMQP:Url"];

                if (string.IsNullOrWhiteSpace(cloudAmqpUrl))
                {
                    throw new InvalidOperationException(
                        "CloudAMQP:Url not configured in appsettings.json");
                }

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(cloudAmqpUrl),
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    RequestedHeartbeat = TimeSpan.FromSeconds(60)
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: QUEUE_NAME,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                _logger.LogInformation("Connected to CloudAMQP successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to CloudAMQP");
                throw;
            }
        }

        public Task PublishSupplierAsync(SupplierDto supplier)
        {
            try
            {
                var json = JsonSerializer.Serialize(supplier);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: QUEUE_NAME,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"Published: {supplier.SupplierName}");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to publish: {supplier.SupplierName}");
                throw;
            }
        }

        public Task PublishSuppliersAsync(List<SupplierDto> suppliers)
        {
            if (suppliers == null || suppliers.Count == 0)
                return Task.CompletedTask;

            try
            {
                foreach (var supplier in suppliers)
                {
                    var json = JsonSerializer.Serialize(supplier);
                    var body = Encoding.UTF8.GetBytes(json);

                    var properties = _channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.DeliveryMode = 2;

                    _channel.BasicPublish(
                        exchange: string.Empty,
                        routingKey: QUEUE_NAME,
                        basicProperties: properties,
                        body: body
                    );
                }

                _logger.LogInformation($"Published {suppliers.Count} suppliers");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish bulk suppliers");
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                _logger.LogInformation("RabbitMQ connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RabbitMQ connection");
            }
        }
    }
}