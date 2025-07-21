using System.ComponentModel.DataAnnotations;
using ECommerce.Common.Models;

namespace PaymentService.Models;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PaymentNumber { get; set; } = string.Empty;
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Currency { get; set; } = "USD";
    
    [Required]
    [MaxLength(20)]
    public string PaymentMethod { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    [MaxLength(100)]
    public string? TransactionId { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(500)]
    public string? FailureReason { get; set; }
    
    public DateTime ProcessedAt { get; set; }
    
    public DateTime? RefundedAt { get; set; }
    
    public decimal? RefundAmount { get; set; }
    
    [MaxLength(500)]
    public string? RefundReason { get; set; }
    
    // Stripe specific fields
    [MaxLength(100)]
    public string? StripePaymentIntentId { get; set; }
    
    [MaxLength(100)]
    public string? StripeCustomerId { get; set; }
    
    [MaxLength(100)]
    public string? StripeRefundId { get; set; }
}

public class PaymentMethod : BaseEntity
{
    public Guid CustomerId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // CreditCard, DebitCard, PayPal, etc.
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? LastFourDigits { get; set; }
    
    [MaxLength(10)]
    public string? ExpiryMonth { get; set; }
    
    [MaxLength(4)]
    public string? ExpiryYear { get; set; }
    
    [MaxLength(100)]
    public string? StripePaymentMethodId { get; set; }
    
    public bool IsDefault { get; set; }
    
    public bool IsActive { get; set; } = true;
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Succeeded,
    Failed,
    Cancelled,
    Refunded,
    PartiallyRefunded
}

public enum PaymentMethodType
{
    CreditCard,
    DebitCard,
    PayPal,
    ApplePay,
    GooglePay,
    BankTransfer
} 