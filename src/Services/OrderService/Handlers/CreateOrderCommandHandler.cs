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

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly OrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        OrderDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Generate order number
        var orderNumber = GenerateOrderNumber();
        
        // Calculate totals
        var totalAmount = request.Items.Sum(item => item.Quantity * 29.99m); // Mock price
        var taxAmount = totalAmount * 0.08m; // 8% tax
        var shippingAmount = 9.99m;
        var discountAmount = 0m;

        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderNumber = orderNumber,
            Status = "Pending",
            TotalAmount = totalAmount + taxAmount + shippingAmount - discountAmount,
            TaxAmount = taxAmount,
            ShippingAmount = shippingAmount,
            DiscountAmount = discountAmount,
            OrderDate = DateTime.UtcNow,
            Notes = request.Notes
        };

        // Create order items
        foreach (var itemDto in request.Items)
        {
            var orderItem = new OrderService.Models.OrderItem
            {
                OrderId = order.Id,
                ProductId = itemDto.ProductId,
                ProductName = $"Product {itemDto.ProductId}", // Mock name
                Quantity = itemDto.Quantity,
                UnitPrice = 29.99m, // Mock price
                TotalPrice = itemDto.Quantity * 29.99m,
                ProductImageUrl = null
            };
            order.Items.Add(orderItem);
        }

        // Create addresses
        order.ShippingAddress = new OrderAddress
        {
            OrderId = order.Id,
            AddressType = "Shipping",
            FirstName = request.ShippingAddress.FirstName,
            LastName = request.ShippingAddress.LastName,
            StreetAddress = request.ShippingAddress.StreetAddress,
            StreetAddress2 = request.ShippingAddress.StreetAddress2,
            City = request.ShippingAddress.City,
            State = request.ShippingAddress.State,
            PostalCode = request.ShippingAddress.PostalCode,
            Country = request.ShippingAddress.Country,
            PhoneNumber = request.ShippingAddress.PhoneNumber,
            Email = request.ShippingAddress.Email
        };

        order.BillingAddress = new OrderAddress
        {
            OrderId = order.Id,
            AddressType = "Billing",
            FirstName = request.BillingAddress.FirstName,
            LastName = request.BillingAddress.LastName,
            StreetAddress = request.BillingAddress.StreetAddress,
            StreetAddress2 = request.BillingAddress.StreetAddress2,
            City = request.BillingAddress.City,
            State = request.BillingAddress.State,
            PostalCode = request.BillingAddress.PostalCode,
            Country = request.BillingAddress.Country,
            PhoneNumber = request.BillingAddress.PhoneNumber,
            Email = request.BillingAddress.Email
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish event
        var orderItems = order.Items.Select(item => new ECommerce.Events.OrderItem
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        }).ToList();

        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            order.CustomerId,
            orderItems,
            order.TotalAmount,
            order.Status);

        await _messageProducer.ProduceAsync("order-events", orderCreatedEvent, cancellationToken);

        _logger.LogInformation("Order created with ID: {OrderId}, Number: {OrderNumber}", order.Id, order.OrderNumber);

        return _mapper.Map<OrderDto>(order);
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
} 