namespace ECommerce.Events;

public class PaymentProcessedEvent : BaseEvent
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }

    public PaymentProcessedEvent(Guid paymentId, Guid orderId, decimal amount, string paymentMethod, string status, string transactionId)
        : base(paymentId.ToString())
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Status = status;
        TransactionId = transactionId;
        ProcessedAt = DateTime.UtcNow;
    }
}

public class PaymentFailedEvent : BaseEvent
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }

    public PaymentFailedEvent(Guid paymentId, Guid orderId, decimal amount, string paymentMethod, string errorMessage)
        : base(paymentId.ToString())
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        ErrorMessage = errorMessage;
        FailedAt = DateTime.UtcNow;
    }
}

public class PaymentRefundedEvent : BaseEvent
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime RefundedAt { get; set; }

    public PaymentRefundedEvent(Guid paymentId, Guid orderId, decimal refundAmount, string reason)
        : base(paymentId.ToString())
    {
        PaymentId = paymentId;
        OrderId = orderId;
        RefundAmount = refundAmount;
        Reason = reason;
        RefundedAt = DateTime.UtcNow;
    }
} 