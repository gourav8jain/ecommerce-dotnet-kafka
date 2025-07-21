using AutoMapper;
using ECommerce.Events;
using ECommerce.Messaging.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentService.Commands;
using PaymentService.Data;
using PaymentService.DTOs;
using PaymentService.Models;
using Stripe;

namespace PaymentService.Handlers;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;

    public CreatePaymentCommandHandler(
        PaymentDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<CreatePaymentCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        // Generate payment number
        var paymentNumber = GeneratePaymentNumber();
        
        var payment = new Payment
        {
            OrderId = request.OrderId,
            PaymentNumber = paymentNumber,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentMethod = request.PaymentMethod,
            Status = "Pending",
            Description = request.Description,
            ProcessedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment created with ID: {PaymentId}, Number: {PaymentNumber}", payment.Id, payment.PaymentNumber);

        return _mapper.Map<PaymentDto>(payment);
    }

    private string GeneratePaymentNumber()
    {
        return $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
}

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentDto>
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        PaymentDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

        if (payment == null)
            throw new ArgumentException("Payment not found");

        try
        {
            // Process payment with Stripe
            var paymentIntentService = new PaymentIntentService();
            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(payment.Amount * 100), // Convert to cents
                Currency = payment.Currency.ToLower(),
                PaymentMethod = request.PaymentMethodId,
                Customer = request.CustomerId,
                Description = payment.Description,
                Confirm = true,
                ReturnUrl = "https://your-domain.com/payment/success"
            };

            var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentOptions);

            // Update payment with Stripe details
            payment.Status = paymentIntent.Status == "succeeded" ? "Succeeded" : "Failed";
            payment.TransactionId = paymentIntent.Id;
            payment.StripePaymentIntentId = paymentIntent.Id;
            payment.ProcessedAt = DateTime.UtcNow;

            if (paymentIntent.Status != "succeeded")
            {
                payment.FailureReason = paymentIntent.LastPaymentError?.Message;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Publish event
            if (payment.Status == "Succeeded")
            {
                var paymentProcessedEvent = new PaymentProcessedEvent(
                    payment.Id,
                    payment.OrderId,
                    payment.Amount,
                    payment.PaymentMethod,
                    payment.Status,
                    payment.TransactionId!);

                await _messageProducer.ProduceAsync("payment-events", paymentProcessedEvent, cancellationToken);
            }
            else
            {
                var paymentFailedEvent = new PaymentFailedEvent(
                    payment.Id,
                    payment.OrderId,
                    payment.Amount,
                    payment.PaymentMethod,
                    payment.FailureReason ?? "Payment processing failed");

                await _messageProducer.ProduceAsync("payment-events", paymentFailedEvent, cancellationToken);
            }

            _logger.LogInformation("Payment processed with ID: {PaymentId}, Status: {Status}", payment.Id, payment.Status);

            return _mapper.Map<PaymentDto>(payment);
        }
        catch (StripeException ex)
        {
            payment.Status = "Failed";
            payment.FailureReason = ex.Message;
            await _context.SaveChangesAsync(cancellationToken);

            var paymentFailedEvent = new PaymentFailedEvent(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                payment.PaymentMethod,
                ex.Message);

            await _messageProducer.ProduceAsync("payment-events", paymentFailedEvent, cancellationToken);

            _logger.LogError(ex, "Payment processing failed for ID: {PaymentId}", payment.Id);
            throw;
        }
    }
}

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentDto>
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<RefundPaymentCommandHandler> _logger;

    public RefundPaymentCommandHandler(
        PaymentDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<RefundPaymentCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

        if (payment == null)
            throw new ArgumentException("Payment not found");

        if (string.IsNullOrEmpty(payment.StripePaymentIntentId))
            throw new ArgumentException("Payment was not processed through Stripe");

        try
        {
            // Process refund with Stripe
            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = payment.StripePaymentIntentId,
                Amount = request.Amount.HasValue ? (long)(request.Amount.Value * 100) : null,
                Reason = request.Reason
            };

            var refund = await refundService.CreateAsync(refundOptions);

            // Update payment with refund details
            payment.RefundAmount = request.Amount ?? payment.Amount;
            payment.RefundReason = request.Reason;
            payment.RefundedAt = DateTime.UtcNow;
            payment.Status = request.Amount.HasValue && request.Amount.Value < payment.Amount ? "PartiallyRefunded" : "Refunded";
            payment.StripeRefundId = refund.Id;

            await _context.SaveChangesAsync(cancellationToken);

            // Publish event
            var paymentRefundedEvent = new PaymentRefundedEvent(
                payment.Id,
                payment.OrderId,
                payment.RefundAmount.Value,
                request.Reason);

            await _messageProducer.ProduceAsync("payment-events", paymentRefundedEvent, cancellationToken);

            _logger.LogInformation("Payment refunded with ID: {PaymentId}, Amount: {RefundAmount}", payment.Id, payment.RefundAmount);

            return _mapper.Map<PaymentDto>(payment);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Payment refund failed for ID: {PaymentId}", payment.Id);
            throw;
        }
    }
} 