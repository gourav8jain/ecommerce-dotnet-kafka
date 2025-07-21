using MediatR;
using PaymentService.DTOs;

namespace PaymentService.Queries;

public class GetPaymentsQuery : IRequest<IEnumerable<PaymentDto>>
{
    public Guid? OrderId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetPaymentByIdQuery : IRequest<PaymentDto?>
{
    public Guid Id { get; set; }
}

public class GetPaymentByNumberQuery : IRequest<PaymentDto?>
{
    public string PaymentNumber { get; set; } = string.Empty;
}

public class GetOrderPaymentsQuery : IRequest<IEnumerable<PaymentDto>>
{
    public Guid OrderId { get; set; }
}

public class GetPaymentMethodsQuery : IRequest<IEnumerable<PaymentMethodDto>>
{
    public Guid CustomerId { get; set; }
    public bool? IsActive { get; set; }
} 