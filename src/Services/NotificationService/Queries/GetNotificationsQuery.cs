using MediatR;
using NotificationService.DTOs;

namespace NotificationService.Queries;

public class GetNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
{
    public Guid? CustomerId { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetNotificationByIdQuery : IRequest<NotificationDto?>
{
    public Guid Id { get; set; }
}

public class GetCustomerNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
{
    public Guid CustomerId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetNotificationTemplatesQuery : IRequest<IEnumerable<NotificationTemplateDto>>
{
    public string? Type { get; set; }
    public bool? IsActive { get; set; }
}

public class GetNotificationTemplateByNameQuery : IRequest<NotificationTemplateDto?>
{
    public string Name { get; set; } = string.Empty;
}
 