using System.ComponentModel.DataAnnotations;
using ECommerce.Common.Models;

namespace NotificationService.Models;

public class Notification : BaseEntity
{
    public Guid CustomerId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string NotificationNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // Email, SMS, Push
    
    [Required]
    [MaxLength(100)]
    public string Subject { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Recipient { get; set; } = string.Empty; // Email address or phone number
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    public DateTime? SentAt { get; set; }
    
    public DateTime? DeliveredAt { get; set; }
    
    [MaxLength(500)]
    public string? FailureReason { get; set; }
    
    public int RetryCount { get; set; }
    
    public DateTime? NextRetryAt { get; set; }
    
    [MaxLength(100)]
    public string? ExternalId { get; set; } // SendGrid message ID, Twilio SID, etc.
    
    [MaxLength(500)]
    public string? Metadata { get; set; } // JSON string for additional data
    
    // Related entities
    public Guid? OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid? ProductId { get; set; }
}

public class NotificationTemplate : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Subject { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(500)]
    public string? Variables { get; set; } // JSON string for template variables
}

public enum NotificationType
{
    Email,
    SMS,
    Push
}

public enum NotificationStatus
{
    Pending,
    Sending,
    Sent,
    Delivered,
    Failed,
    Cancelled
}

public enum NotificationTemplateType
{
    OrderConfirmation,
    OrderShipped,
    OrderDelivered,
    PaymentSuccess,
    PaymentFailed,
    PaymentRefunded,
    ProductBackInStock,
    Welcome,
    PasswordReset,
    AccountVerification
} 