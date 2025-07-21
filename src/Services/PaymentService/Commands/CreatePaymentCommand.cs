using MediatR;
using PaymentService.DTOs;

namespace PaymentService.Commands;

public class CreatePaymentCommand : IRequest<PaymentDto>
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CreatePaymentMethodDto? PaymentMethodDetails { get; set; }
}

public class ProcessPaymentCommand : IRequest<PaymentDto>
{
    public Guid PaymentId { get; set; }
    public string PaymentMethodId { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
}

public class RefundPaymentCommand : IRequest<PaymentDto>
{
    public Guid PaymentId { get; set; }
    public decimal? Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class CreatePaymentMethodCommand : IRequest<PaymentMethodDto>
{
    public Guid CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LastFourDigits { get; set; }
    public string? ExpiryMonth { get; set; }
    public string? ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
}

public class CreateStripePaymentIntentCommand : IRequest<StripePaymentIntentDto>
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? PaymentMethodId { get; set; }
    public string? CustomerId { get; set; }
    public string? Description { get; set; }
} 