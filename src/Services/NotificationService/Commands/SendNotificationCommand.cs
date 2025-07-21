using MediatR;
using NotificationService.DTOs;

namespace NotificationService.Commands;

public class SendNotificationCommand : IRequest<NotificationDto>
{
    public Guid CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid? ProductId { get; set; }
    public string? Metadata { get; set; }
}

public class SendTemplateNotificationCommand : IRequest<NotificationDto>
{
    public Guid CustomerId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public Dictionary<string, string>? Variables { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid? ProductId { get; set; }
}

public class CreateNotificationTemplateCommand : IRequest<NotificationTemplateDto>
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Variables { get; set; }
}

public class UpdateNotificationStatusCommand : IRequest<bool>
{
    public Guid NotificationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public string? FailureReason { get; set; }
} 