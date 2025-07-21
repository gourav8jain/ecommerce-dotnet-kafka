using ECommerce.Events;

namespace ECommerce.Messaging.Interfaces;

public interface IMessageProducer
{
    Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : BaseEvent;
    Task ProduceAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default) where T : BaseEvent;
    Task ProduceBatchAsync<T>(string topic, IEnumerable<T> messages, CancellationToken cancellationToken = default) where T : BaseEvent;
} 