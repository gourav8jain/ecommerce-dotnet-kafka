using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.DTOs;
using NotificationService.Queries;

namespace NotificationService.Handlers;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly NotificationDbContext _context;
    private readonly IMapper _mapper;

    public GetNotificationsQueryHandler(NotificationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Notifications.AsQueryable();

        // Apply filters
        if (request.CustomerId.HasValue)
            query = query.Where(n => n.CustomerId == request.CustomerId.Value);

        if (!string.IsNullOrEmpty(request.Type))
            query = query.Where(n => n.Type == request.Type);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(n => n.Status == request.Status);

        if (request.FromDate.HasValue)
            query = query.Where(n => n.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(n => n.CreatedAt <= request.ToDate.Value);

        // Apply pagination
        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }
}

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, NotificationDto?>
{
    private readonly NotificationDbContext _context;
    private readonly IMapper _mapper;

    public GetNotificationByIdQueryHandler(NotificationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<NotificationDto?> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);

        return _mapper.Map<NotificationDto>(notification);
    }
}

public class GetCustomerNotificationsQueryHandler : IRequestHandler<GetCustomerNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly NotificationDbContext _context;
    private readonly IMapper _mapper;

    public GetCustomerNotificationsQueryHandler(NotificationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetCustomerNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _context.Notifications
            .Where(n => n.CustomerId == request.CustomerId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }
}

public class GetNotificationTemplatesQueryHandler : IRequestHandler<GetNotificationTemplatesQuery, IEnumerable<NotificationTemplateDto>>
{
    private readonly NotificationDbContext _context;
    private readonly IMapper _mapper;

    public GetNotificationTemplatesQueryHandler(NotificationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationTemplateDto>> Handle(GetNotificationTemplatesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.NotificationTemplates.AsQueryable();

        if (!string.IsNullOrEmpty(request.Type))
            query = query.Where(nt => nt.Type == request.Type);

        if (request.IsActive.HasValue)
            query = query.Where(nt => nt.IsActive == request.IsActive.Value);

        var templates = await query
            .OrderBy(nt => nt.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }
}

public class GetNotificationTemplateByNameQueryHandler : IRequestHandler<GetNotificationTemplateByNameQuery, NotificationTemplateDto?>
{
    private readonly NotificationDbContext _context;
    private readonly IMapper _mapper;

    public GetNotificationTemplateByNameQueryHandler(NotificationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<NotificationTemplateDto?> Handle(GetNotificationTemplateByNameQuery request, CancellationToken cancellationToken)
    {
        var template = await _context.NotificationTemplates
            .FirstOrDefaultAsync(nt => nt.Name == request.Name, cancellationToken);

        return _mapper.Map<NotificationTemplateDto>(template);
    }
} 