using AdminTemplate.DTOs;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Channels;
using System.Linq;
using System.Collections.Generic;

namespace AdminTemplate.Services
{
    public class SupplierQueueService : BackgroundService, ISupplierQueueService
    {
        private readonly Channel<SupplierDto> _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SupplierQueueService> _logger;
        private int _processedCount = 0;
        private int _failedCount = 0;
        private int _queuedCount = 0;
        private readonly ConcurrentBag<string> _errors = new ConcurrentBag<string>();
        private readonly SemaphoreSlim _maxConcurrency;
        private const int MAX_CONCURRENT_WORKERS = 5; // Process 5 items simultaneously
        private const int MAX_QUEUE_CAPACITY = 10000; // Max items in queue

        public SupplierQueueService(
            IServiceProvider serviceProvider,
            ILogger<SupplierQueueService> logger)
        {
            // Use Channel for better performance with multiple producers/consumers
            var options = new BoundedChannelOptions(MAX_QUEUE_CAPACITY)
            {
                FullMode = BoundedChannelFullMode.Wait // Wait if queue is full
            };
            _channel = Channel.CreateBounded<SupplierDto>(options);

            _serviceProvider = serviceProvider;
            _logger = logger;
            _maxConcurrency = new SemaphoreSlim(MAX_CONCURRENT_WORKERS);
        }

        public async Task<bool> QueueSupplierAsync(SupplierDto supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            try
            {
                await _channel.Writer.WriteAsync(supplier);
                Interlocked.Increment(ref _queuedCount);
                _logger.LogInformation($"Supplier '{supplier.SupplierName}' queued. Total queued: {_queuedCount}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to queue supplier: {supplier.SupplierName}");
                return false;
            }
        }

        public async Task<bool> QueueSuppliersAsync(List<SupplierDto> suppliers)
        {
            if (suppliers == null || suppliers.Count == 0)
                return false;

            try
            {
                foreach (var supplier in suppliers)
                {
                    await _channel.Writer.WriteAsync(supplier);
                    Interlocked.Increment(ref _queuedCount);
                }

                _logger.LogInformation($"{suppliers.Count} suppliers queued. Total in queue: {_queuedCount}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue bulk suppliers");
                return false;
            }
        }

        public QueueStatus GetQueueStatus()
        {
            return new QueueStatus
            {
                QueuedCount = _queuedCount - _processedCount - _failedCount,
                ProcessedCount = _processedCount,
                FailedCount = _failedCount,
                TotalQueued = _queuedCount,
                RecentErrors = _errors.TakeLast(20).ToList(),
                MaxConcurrentWorkers = MAX_CONCURRENT_WORKERS,
                MaxQueueCapacity = MAX_QUEUE_CAPACITY
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Supplier Queue Service started with {MAX_CONCURRENT_WORKERS} concurrent workers.");

            // Create multiple worker tasks for parallel processing
            var workers = new List<Task>();
            for (int i = 0; i < MAX_CONCURRENT_WORKERS; i++)
            {
                int workerId = i + 1;
                workers.Add(ProcessQueueAsync(workerId, stoppingToken));
            }

            // Wait for all workers to complete
            await Task.WhenAll(workers);

            _logger.LogInformation("Supplier Queue Service stopped.");
        }

        private async Task ProcessQueueAsync(int workerId, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Worker {workerId} started.");

            await foreach (var supplier in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    // Semaphore ensures we don't exceed max concurrent DB operations
                    await _maxConcurrency.WaitAsync(stoppingToken);

                    try
                    {
                        await ProcessSupplierAsync(supplier, workerId);
                    }
                    finally
                    {
                        _maxConcurrency.Release();
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Worker {workerId}: Error in processing loop");
                }
            }

            _logger.LogInformation($"Worker {workerId} stopped.");
        }

        private async Task ProcessSupplierAsync(SupplierDto supplier, int workerId)
        {
            using var scope = _serviceProvider.CreateScope();
            var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            try
            {
                _logger.LogDebug($"Worker {workerId}: Processing '{supplier.SupplierName}'");

                await supplierService.AddAsync(supplier);

                Interlocked.Increment(ref _processedCount);

                _logger.LogInformation(
                    $"Worker {workerId}: ✓ Successfully processed '{supplier.SupplierName}' " +
                    $"(Total: {_processedCount}/{_queuedCount})");
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedCount);

                var errorMsg = $"Worker {workerId}: Failed '{supplier.SupplierName}': {ex.Message}";
                _errors.Add($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {errorMsg}");

                _logger.LogError(ex, errorMsg);

                // Keep only last 200 errors
                while (_errors.Count > 200)
                {
                    _errors.TryTake(out _);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping queue service. Processing remaining items...");

            // Signal no more writes
            _channel.Writer.Complete();

            await base.StopAsync(cancellationToken);

            var remaining = _queuedCount - _processedCount - _failedCount;
            _logger.LogInformation(
                $"Queue service stopped. Processed: {_processedCount}, " +
                $"Failed: {_failedCount}, Remaining: {remaining}");
        }
    }
}