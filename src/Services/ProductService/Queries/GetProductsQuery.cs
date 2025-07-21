using MediatR;
using ProductService.DTOs;

namespace ProductService.Queries;

public class GetProductsQuery : IRequest<IEnumerable<ProductDto>>
{
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public Guid Id { get; set; }
}

public class GetProductsByCategoryQuery : IRequest<IEnumerable<ProductDto>>
{
    public string Category { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SearchProductsQuery : IRequest<IEnumerable<ProductDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
} 