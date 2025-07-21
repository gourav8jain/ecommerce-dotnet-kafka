using AutoMapper;
using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Order mappings
        CreateMap<Order, OrderDto>();
        CreateMap<CreateOrderDto, Order>();
        CreateMap<UpdateOrderStatusDto, Order>();
        
        // OrderItem mappings
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<CreateOrderItemDto, OrderItem>();
        
        // OrderAddress mappings
        CreateMap<OrderAddress, OrderAddressDto>();
        CreateMap<CreateOrderAddressDto, OrderAddress>();
    }
} 