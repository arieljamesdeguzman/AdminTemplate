using AdminTemplate.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private readonly IConfiguration _configuration;
        private const string QUEUE_NAME = "supplier_queue";
        private const ushort CONCURRENT_WORKERS = 5;
        private int _processedCount = 0;
        private int _failedCount = 0;

        public RabbitMQConsumerService(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<RabbitMQConsumerService> logger)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var cloudAmqpUrl = _configuration["CloudAMQP:Url"];

                if (string.IsNullOrWhiteSpace(cloudAmqpUrl))
                {
                    _logger.LogError("CloudAMQP:Url not configured in appsettings.json");
                    return;
                }

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(cloudAmqpUrl),
                    DispatchConsumersAsync = true,
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

                _channel.BasicQos(prefetchSize: 0, prefetchCount: CONCURRENT_WORKERS, global: false);

                _logger.LogInformation($"Consumer connected with {CONCURRENT_WORKERS} workers");

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var supplier = JsonSerializer.Deserialize<SupplierDto>(json);

                        _logger.LogInformation($"Processing: {supplier.SupplierName}");

                        using var scope = _serviceProvider.CreateScope();
                        var supplierService = scope.ServiceProvider
                            .GetRequiredService<ISupplierService>();

                        await supplierService.AddAsync(supplier);

                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                        Interlocked.Increment(ref _processedCount);
                        _logger.LogInformation(
                            $"Successfully processed: {supplier.SupplierName} " +
                            $"(Total: {_processedCount}, Failed: {_failedCount})");
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref _failedCount);
                        _logger.LogError(ex, $"Failed to process message. Total failures: {_failedCount}");

                        _channel.BasicNack(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false,
                            requeue: true
                        );
                    }
                };

                _channel.BasicConsume(
                    queue: QUEUE_NAME,
                    autoAck: false,
                    consumer: consumer
                );

                _logger.LogInformation("RabbitMQ Consumer Service started");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start consumer service");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                $"Stopping Consumer Service. Processed: {_processedCount}, Failed: {_failedCount}");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                _logger.LogInformation("Consumer connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing consumer connection");
            }

            base.Dispose();
        }
    }
}