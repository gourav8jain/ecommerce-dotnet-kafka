using System.Text.Json.Serialization;

namespace ECommerce.Events;

public abstract class BaseEvent
{
    public Guid EventId { get; set; }
    public DateTime OccurredOn { get; set; }
    public string EventType { get; set; }
    public string? AggregateId { get; set; }
    public long Version { get; set; }

    protected BaseEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().Name;
        Version = 1;
    }

    protected BaseEvent(string aggregateId) : this()
    {
        AggregateId = aggregateId;
    }
} 