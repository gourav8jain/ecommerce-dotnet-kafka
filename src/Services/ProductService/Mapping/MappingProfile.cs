using AutoMapper;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        
        // ProductReview mappings
        CreateMap<ProductReview, ProductReviewDto>();
        CreateMap<CreateProductReviewDto, ProductReview>();
        CreateMap<UpdateProductReviewDto, ProductReview>();
    }
} 