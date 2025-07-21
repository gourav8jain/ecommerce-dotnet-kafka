using MediatR;
using OrderService.DTOs;

namespace OrderService.Commands;

public class CreateOrderCommand : IRequest<OrderDto>
{
    public Guid CustomerId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public CreateOrderAddressDto ShippingAddress { get; set; } = null!;
    public CreateOrderAddressDto BillingAddress { get; set; } = null!;
    public string? Notes { get; set; }
}

public class UpdateOrderStatusCommand : IRequest<OrderDto>
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CancelOrderCommand : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ShipOrderCommand : IRequest<OrderDto>
{
    public Guid OrderId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
}

public class DeliverOrderCommand : IRequest<OrderDto>
{
    public Guid OrderId { get; set; }
} 