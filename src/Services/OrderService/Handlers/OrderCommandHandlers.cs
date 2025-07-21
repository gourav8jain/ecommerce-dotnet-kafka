using AutoMapper;
using ECommerce.Events;
using ECommerce.Messaging.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Commands;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Handlers;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly OrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        OrderDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken);

        if (order == null)
            throw new ArgumentException("Order not found");

        var oldStatus = order.Status;
        order.Status = request.Status;
        order.Notes = request.Notes ?? order.Notes;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Publish event
        var orderStatusUpdatedEvent = new OrderStatusUpdatedEvent(order.Id, oldStatus, order.Status);
        await _messageProducer.ProduceAsync("order-status-updated", orderStatusUpdatedEvent);

        _logger.LogInformation("Order status updated for ID: {OrderId}, Status: {OldStatus} -> {NewStatus}", 
            order.Id, oldStatus, order.Status);

        return _mapper.Map<OrderDto>(order);
    }
}

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly OrderDbContext _context;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(
        OrderDbContext context,
        IMessageProducer messageProducer,
        ILogger<CancelOrderCommandHandler> logger)
    {
        _context = context;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken);

        if (order == null)
            throw new ArgumentException("Order not found");

        order.Status = "Cancelled";
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Publish event
        var orderCancelledEvent = new OrderCancelledEvent(order.Id, request.Reason);
        await _messageProducer.ProduceAsync("order-cancelled", orderCancelledEvent);

        _logger.LogInformation("Order cancelled with ID: {OrderId}, Reason: {Reason}", order.Id, request.Reason);

        return true;
    }
}

public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, OrderDto>
{
    private readonly OrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<ShipOrderCommandHandler> _logger;

    public ShipOrderCommandHandler(
        OrderDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<ShipOrderCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken);

        if (order == null)
            throw new ArgumentException("Order not found");

        var oldStatus = order.Status;
        order.Status = "Shipped";
        order.ShippedDate = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Publish event
        var orderStatusUpdatedEvent = new OrderStatusUpdatedEvent(order.Id, oldStatus, order.Status);
        await _messageProducer.ProduceAsync("order-status-updated", orderStatusUpdatedEvent);

        _logger.LogInformation("Order shipped with ID: {OrderId}, Tracking: {TrackingNumber}", 
            order.Id, request.TrackingNumber);

        return _mapper.Map<OrderDto>(order);
    }
}

public class DeliverOrderCommandHandler : IRequestHandler<DeliverOrderCommand, OrderDto>
{
    private readonly OrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<DeliverOrderCommandHandler> _logger;

    public DeliverOrderCommandHandler(
        OrderDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<DeliverOrderCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken);

        if (order == null)
            throw new ArgumentException("Order not found");

        var oldStatus = order.Status;
        order.Status = "Delivered";
        order.DeliveredDate = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Publish event
        var orderStatusUpdatedEvent = new OrderStatusUpdatedEvent(order.Id, oldStatus, order.Status);
        await _messageProducer.ProduceAsync("order-status-updated", orderStatusUpdatedEvent);

        _logger.LogInformation("Order delivered with ID: {OrderId}", order.Id);

        return _mapper.Map<OrderDto>(order);
    }
} 