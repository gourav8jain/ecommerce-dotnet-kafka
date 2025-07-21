using AutoMapper;
using ECommerce.Events;
using ECommerce.Messaging.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Commands;
using ProductService.Data;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly ProductDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        ProductDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Category = request.Category,
            ImageUrl = request.ImageUrl,
            Brand = request.Brand
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish event
        var productCreatedEvent = new ProductCreatedEvent(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.Category);

        await _messageProducer.ProduceAsync("product-events", productCreatedEvent, cancellationToken);

        _logger.LogInformation("Product created with ID: {ProductId}", product.Id);

        return _mapper.Map<ProductDto>(product);
    }
} 