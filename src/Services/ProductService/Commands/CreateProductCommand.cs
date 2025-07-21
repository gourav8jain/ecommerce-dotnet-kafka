using MediatR;
using ProductService.DTOs;

namespace ProductService.Commands;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
}

public class UpdateProductCommand : IRequest<ProductDto>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public bool? IsActive { get; set; }
}

public class DeleteProductCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class UpdateProductStockCommand : IRequest<ProductDto>
{
    public Guid Id { get; set; }
    public int NewStockQuantity { get; set; }
} 