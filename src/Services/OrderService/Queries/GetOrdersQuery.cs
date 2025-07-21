using MediatR;
using OrderService.DTOs;

namespace OrderService.Queries;

public class GetOrdersQuery : IRequest<IEnumerable<OrderDto>>
{
    public Guid? CustomerId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid Id { get; set; }
}

public class GetOrderByNumberQuery : IRequest<OrderDto?>
{
    public string OrderNumber { get; set; } = string.Empty;
}

public class GetCustomerOrdersQuery : IRequest<IEnumerable<OrderDto>>
{
    public Guid CustomerId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
} 