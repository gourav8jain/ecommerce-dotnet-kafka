using ECommerce.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Commands;
using PaymentService.DTOs;
using PaymentService.Queries;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentDto>>>> GetPayments(
        [FromQuery] Guid? orderId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetPaymentsQuery
            {
                OrderId = orderId,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var payments = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResult(payments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            return StatusCode(500, ApiResponse<IEnumerable<PaymentDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> GetPayment(Guid id)
    {
        try
        {
            var query = new GetPaymentByIdQuery { Id = id };
            var payment = await _mediator.Send(query);

            if (payment == null)
                return NotFound(ApiResponse<PaymentDto>.ErrorResult("Payment not found", statusCode: 404));

            return Ok(ApiResponse<PaymentDto>.SuccessResult(payment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment with ID: {PaymentId}", id);
            return StatusCode(500, ApiResponse<PaymentDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("number/{paymentNumber}")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> GetPaymentByNumber(string paymentNumber)
    {
        try
        {
            var query = new GetPaymentByNumberQuery { PaymentNumber = paymentNumber };
            var payment = await _mediator.Send(query);

            if (payment == null)
                return NotFound(ApiResponse<PaymentDto>.ErrorResult("Payment not found", statusCode: 404));

            return Ok(ApiResponse<PaymentDto>.SuccessResult(payment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment with number: {PaymentNumber}", paymentNumber);
            return StatusCode(500, ApiResponse<PaymentDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentDto>>>> GetOrderPayments(Guid orderId)
    {
        try
        {
            var query = new GetOrderPaymentsQuery { OrderId = orderId };
            var payments = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResult(payments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for order: {OrderId}", orderId);
            return StatusCode(500, ApiResponse<IEnumerable<PaymentDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
    {
        try
        {
            var command = new CreatePaymentCommand
            {
                OrderId = createPaymentDto.OrderId,
                Amount = createPaymentDto.Amount,
                Currency = createPaymentDto.Currency,
                PaymentMethod = createPaymentDto.PaymentMethod,
                Description = createPaymentDto.Description,
                PaymentMethodDetails = createPaymentDto.PaymentMethodDetails
            };

            var payment = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, 
                ApiResponse<PaymentDto>.SuccessResult(payment, "Payment created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            return StatusCode(500, ApiResponse<PaymentDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("{id:guid}/process")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> ProcessPayment(Guid id, [FromBody] ProcessPaymentDto processPaymentDto)
    {
        try
        {
            var command = new ProcessPaymentCommand
            {
                PaymentId = id,
                PaymentMethodId = processPaymentDto.PaymentMethodId,
                CustomerId = processPaymentDto.CustomerId
            };

            var payment = await _mediator.Send(command);
            return Ok(ApiResponse<PaymentDto>.SuccessResult(payment, "Payment processed successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<PaymentDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment with ID: {PaymentId}", id);
            return StatusCode(500, ApiResponse<PaymentDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("{id:guid}/refund")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> RefundPayment(Guid id, [FromBody] RefundPaymentDto refundPaymentDto)
    {
        try
        {
            var command = new RefundPaymentCommand
            {
                PaymentId = id,
                Amount = refundPaymentDto.Amount,
                Reason = refundPaymentDto.Reason
            };

            var payment = await _mediator.Send(command);
            return Ok(ApiResponse<PaymentDto>.SuccessResult(payment, "Payment refunded successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<PaymentDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment with ID: {PaymentId}", id);
            return StatusCode(500, ApiResponse<PaymentDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("stripe/payment-intent")]
    public async Task<ActionResult<ApiResponse<StripePaymentIntentDto>>> CreateStripePaymentIntent([FromBody] CreatePaymentDto createPaymentDto)
    {
        try
        {
            var command = new CreateStripePaymentIntentCommand
            {
                Amount = createPaymentDto.Amount,
                Currency = createPaymentDto.Currency,
                PaymentMethodId = null, // Will be set by client
                CustomerId = null, // Will be set by client
                Description = createPaymentDto.Description
            };

            var paymentIntent = await _mediator.Send(command);
            return Ok(ApiResponse<StripePaymentIntentDto>.SuccessResult(paymentIntent, "Payment intent created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Stripe payment intent");
            return StatusCode(500, ApiResponse<StripePaymentIntentDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("methods/customer/{customerId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentMethodDto>>>> GetCustomerPaymentMethods(
        Guid customerId,
        [FromQuery] bool? isActive)
    {
        try
        {
            var query = new GetPaymentMethodsQuery
            {
                CustomerId = customerId,
                IsActive = isActive
            };

            var paymentMethods = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResult(paymentMethods));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment methods for customer: {CustomerId}", customerId);
            return StatusCode(500, ApiResponse<IEnumerable<PaymentMethodDto>>.ErrorResult("Internal server error"));
        }
    }
} 