using ECommerce.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Commands;
using OrderService.DTOs;
using OrderService.Queries;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders(
        [FromQuery] Guid? customerId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetOrdersQuery
            {
                CustomerId = customerId,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var orders = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(Guid id)
    {
        try
        {
            var query = new GetOrderByIdQuery { Id = id };
            var order = await _mediator.Send(query);

            if (order == null)
                return NotFound(ApiResponse<OrderDto>.ErrorResult("Order not found", statusCode: 404));

            return Ok(ApiResponse<OrderDto>.SuccessResult(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with ID: {OrderId}", id);
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("number/{orderNumber}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderByNumber(string orderNumber)
    {
        try
        {
            var query = new GetOrderByNumberQuery { OrderNumber = orderNumber };
            var order = await _mediator.Send(query);

            if (order == null)
                return NotFound(ApiResponse<OrderDto>.ErrorResult("Order not found", statusCode: 404));

            return Ok(ApiResponse<OrderDto>.SuccessResult(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with number: {OrderNumber}", orderNumber);
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetCustomerOrders(
        Guid customerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetCustomerOrdersQuery
            {
                CustomerId = customerId,
                Page = page,
                PageSize = pageSize
            };

            var orders = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for customer: {CustomerId}", customerId);
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            var command = new CreateOrderCommand
            {
                CustomerId = createOrderDto.CustomerId,
                Items = createOrderDto.Items,
                ShippingAddress = createOrderDto.ShippingAddress,
                BillingAddress = createOrderDto.BillingAddress,
                Notes = createOrderDto.Notes
            };

            var order = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, 
                ApiResponse<OrderDto>.SuccessResult(order, "Order created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        try
        {
            var command = new UpdateOrderStatusCommand
            {
                OrderId = id,
                Status = updateStatusDto.Status,
                Notes = updateStatusDto.Notes
            };

            var order = await _mediator.Send(command);
            return Ok(ApiResponse<OrderDto>.SuccessResult(order, "Order status updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status for ID: {OrderId}", id);
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(Guid id, [FromBody] string reason)
    {
        try
        {
            var command = new CancelOrderCommand
            {
                OrderId = id,
                Reason = reason
            };

            var result = await _mediator.Send(command);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Order cancelled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order with ID: {OrderId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("{id:guid}/ship")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> ShipOrder(Guid id, [FromBody] string trackingNumber)
    {
        try
        {
            var command = new ShipOrderCommand
            {
                OrderId = id,
                TrackingNumber = trackingNumber
            };

            var order = await _mediator.Send(command);
            return Ok(ApiResponse<OrderDto>.SuccessResult(order, "Order shipped successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shipping order with ID: {OrderId}", id);
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("{id:guid}/deliver")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> DeliverOrder(Guid id)
    {
        try
        {
            var command = new DeliverOrderCommand
            {
                OrderId = id
            };

            var order = await _mediator.Send(command);
            return Ok(ApiResponse<OrderDto>.SuccessResult(order, "Order delivered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering order with ID: {OrderId}", id);
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult("Internal server error"));
        }
    }
} 