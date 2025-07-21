using AutoMapper;
using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Notification mappings
        CreateMap<Notification, NotificationDto>();
        CreateMap<CreateNotificationDto, Notification>();
        CreateMap<SendNotificationDto, Notification>();
        
        // NotificationTemplate mappings
        CreateMap<NotificationTemplate, NotificationTemplateDto>();
        CreateMap<CreateNotificationTemplateDto, NotificationTemplate>();
    }
} 