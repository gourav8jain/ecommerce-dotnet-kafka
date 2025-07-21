using Confluent.Kafka;
using ECommerce.Common.Models;
using ECommerce.Events;
using ECommerce.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ECommerce.Messaging.Kafka;

public class KafkaProducer : IMessageProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            ClientId = "ecommerce-producer",
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000,
            LingerMs = 5,
            BatchSize = 16384,
            CompressionType = CompressionType.Snappy
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : BaseEvent
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = message.AggregateId ?? Guid.NewGuid().ToString(),
                Value = jsonMessage
            };

            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            
            _logger.LogInformation("Message produced to topic {Topic} at partition {Partition} with offset {Offset}", 
                deliveryResult.Topic, deliveryResult.Partition, deliveryResult.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing message to topic {Topic}", topic);
            throw;
        }
    }

    public async Task ProduceAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default) where T : BaseEvent
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = jsonMessage
            };

            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            
            _logger.LogInformation("Message produced to topic {Topic} at partition {Partition} with offset {Offset}", 
                deliveryResult.Topic, deliveryResult.Partition, deliveryResult.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing message to topic {Topic}", topic);
            throw;
        }
    }

    public async Task ProduceBatchAsync<T>(string topic, IEnumerable<T> messages, CancellationToken cancellationToken = default) where T : BaseEvent
    {
        try
        {
            var tasks = messages.Select(async message =>
            {
                var jsonMessage = JsonSerializer.Serialize(message);
                var kafkaMessage = new Message<string, string>
                {
                    Key = message.AggregateId ?? Guid.NewGuid().ToString(),
                    Value = jsonMessage
                };

                return await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            });

            var results = await Task.WhenAll(tasks);
            
            _logger.LogInformation("Batch of {Count} messages produced to topic {Topic}", 
                results.Length, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing batch messages to topic {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
} 