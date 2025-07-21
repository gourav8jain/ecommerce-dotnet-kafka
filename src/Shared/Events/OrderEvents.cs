using ECommerce.Common.Models;

namespace ECommerce.Events;

public class OrderCreatedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }

    public OrderCreatedEvent(Guid orderId, Guid customerId, List<OrderItem> items, decimal totalAmount, string status)
        : base(orderId.ToString())
    {
        OrderId = orderId;
        CustomerId = customerId;
        Items = items;
        TotalAmount = totalAmount;
        Status = status;
        OrderDate = DateTime.UtcNow;
    }
}

public class OrderStatusUpdatedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }

    public OrderStatusUpdatedEvent(Guid orderId, string oldStatus, string newStatus)
        : base(orderId.ToString())
    {
        OrderId = orderId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class OrderCancelledEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }

    public OrderCancelledEvent(Guid orderId, string reason)
        : base(orderId.ToString())
    {
        OrderId = orderId;
        Reason = reason;
        CancelledAt = DateTime.UtcNow;
    }
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
} 