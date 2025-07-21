using AutoMapper;
using PaymentService.DTOs;
using PaymentService.Models;

namespace PaymentService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Payment mappings
        CreateMap<Payment, PaymentDto>();
        CreateMap<CreatePaymentDto, Payment>();
        CreateMap<ProcessPaymentDto, Payment>();
        CreateMap<RefundPaymentDto, Payment>();
        
        // PaymentMethod mappings
        CreateMap<PaymentMethod, PaymentMethodDto>();
        CreateMap<CreatePaymentMethodDto, PaymentMethod>();
    }
} 