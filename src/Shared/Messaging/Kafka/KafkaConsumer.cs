using Confluent.Kafka;
using ECommerce.Events;
using ECommerce.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ECommerce.Messaging.Kafka;

public class KafkaConsumer : IMessageConsumer, IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumer> _logger;
    private bool _disposed;

    public KafkaConsumer(IConfiguration configuration, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
        
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = configuration["Kafka:GroupId"] ?? "ecommerce-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnablePartitionEof = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public async Task StartConsumingAsync<T>(string topic, Func<T, Task> messageHandler, CancellationToken cancellationToken = default) where T : BaseEvent
    {
        try
        {
            _consumer.Subscribe(topic);
            _logger.LogInformation("Started consuming from topic {Topic}", topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    
                    if (consumeResult.IsPartitionEOF)
                    {
                        _logger.LogDebug("Reached end of partition {Partition}", consumeResult.Partition);
                        continue;
                    }

                    _logger.LogDebug("Received message from topic {Topic} partition {Partition} offset {Offset}", 
                        consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);

                    var message = JsonSerializer.Deserialize<T>(consumeResult.Message.Value);
                    if (message != null)
                    {
                        await messageHandler(message);
                        _consumer.Commit(consumeResult);
                        _logger.LogDebug("Message processed and committed");
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from topic {Topic}", topic);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing message from topic {Topic}", topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while consuming from topic {Topic}", topic);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consuming stopped for topic {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in consumer for topic {Topic}", topic);
            throw;
        }
    }

    public Task StopConsumingAsync()
    {
        try
        {
            _consumer?.Close();
            _logger.LogInformation("Consumer stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping consumer");
        }
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _consumer?.Dispose();
            _disposed = true;
        }
    }
} 