using ECommerce.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Commands;
using ProductService.DTOs;
using ProductService.Queries;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProducts(
        [FromQuery] string? category,
        [FromQuery] string? brand,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetProductsQuery
            {
                Category = category,
                Brand = brand,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                IsActive = isActive,
                Page = page,
                PageSize = pageSize
            };

            var products = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResult(products));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(Guid id)
    {
        try
        {
            var query = new GetProductByIdQuery { Id = id };
            var product = await _mediator.Send(query);

            if (product == null)
                return NotFound(ApiResponse<ProductDto>.ErrorResult("Product not found", statusCode: 404));

            return Ok(ApiResponse<ProductDto>.SuccessResult(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsByCategory(
        string category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetProductsByCategoryQuery
            {
                Category = category,
                Page = page,
                PageSize = pageSize
            };

            var products = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResult(products));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products by category: {Category}", category);
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> SearchProducts(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new SearchProductsQuery
            {
                SearchTerm = searchTerm,
                Page = page,
                PageSize = pageSize
            };

            var products = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResult(products));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            var command = new CreateProductCommand
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                StockQuantity = createProductDto.StockQuantity,
                Category = createProductDto.Category,
                ImageUrl = createProductDto.ImageUrl,
                Brand = createProductDto.Brand
            };

            var product = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, 
                ApiResponse<ProductDto>.SuccessResult(product, "Product created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            var command = new UpdateProductCommand
            {
                Id = id,
                Name = updateProductDto.Name,
                Description = updateProductDto.Description,
                Price = updateProductDto.Price,
                StockQuantity = updateProductDto.StockQuantity,
                Category = updateProductDto.Category,
                ImageUrl = updateProductDto.ImageUrl,
                Brand = updateProductDto.Brand,
                IsActive = updateProductDto.IsActive
            };

            var product = await _mediator.Send(command);
            return Ok(ApiResponse<ProductDto>.SuccessResult(product, "Product updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(Guid id)
    {
        try
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Product not found", statusCode: 404));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Product deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    [HttpPatch("{id:guid}/stock")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProductStock(Guid id, [FromBody] int newStockQuantity)
    {
        try
        {
            var command = new UpdateProductStockCommand
            {
                Id = id,
                NewStockQuantity = newStockQuantity
            };

            var product = await _mediator.Send(command);
            return Ok(ApiResponse<ProductDto>.SuccessResult(product, "Product stock updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product stock for ID: {ProductId}", id);
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Internal server error"));
        }
    }
} 