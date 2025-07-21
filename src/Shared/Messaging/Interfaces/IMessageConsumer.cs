using ECommerce.Events;

namespace ECommerce.Messaging.Interfaces;

public interface IMessageConsumer
{
    Task StartConsumingAsync<T>(string topic, Func<T, Task> messageHandler, CancellationToken cancellationToken = default) where T : BaseEvent;
    Task StopConsumingAsync();
} 