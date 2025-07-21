using ECommerce.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Commands;
using NotificationService.DTOs;
using NotificationService.Queries;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(IMediator mediator, ILogger<NotificationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetNotifications(
        [FromQuery] Guid? customerId,
        [FromQuery] string? type,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetNotificationsQuery
            {
                CustomerId = customerId,
                Type = type,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var notifications = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResult(notifications));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications");
            return StatusCode(500, ApiResponse<IEnumerable<NotificationDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> GetNotification(Guid id)
    {
        try
        {
            var query = new GetNotificationByIdQuery { Id = id };
            var notification = await _mediator.Send(query);

            if (notification == null)
                return NotFound(ApiResponse<NotificationDto>.ErrorResult("Notification not found", statusCode: 404));

            return Ok(ApiResponse<NotificationDto>.SuccessResult(notification));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification with ID: {NotificationId}", id);
            return StatusCode(500, ApiResponse<NotificationDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetCustomerNotifications(
        Guid customerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetCustomerNotificationsQuery
            {
                CustomerId = customerId,
                Page = page,
                PageSize = pageSize
            };

            var notifications = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResult(notifications));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for customer: {CustomerId}", customerId);
            return StatusCode(500, ApiResponse<IEnumerable<NotificationDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> SendNotification([FromBody] CreateNotificationDto createNotificationDto)
    {
        try
        {
            var command = new SendNotificationCommand
            {
                CustomerId = createNotificationDto.CustomerId,
                Type = createNotificationDto.Type,
                Subject = createNotificationDto.Subject,
                Content = createNotificationDto.Content,
                Recipient = createNotificationDto.Recipient,
                OrderId = createNotificationDto.OrderId,
                PaymentId = createNotificationDto.PaymentId,
                ProductId = createNotificationDto.ProductId,
                Metadata = createNotificationDto.Metadata
            };

            var notification = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, 
                ApiResponse<NotificationDto>.SuccessResult(notification, "Notification sent successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<NotificationDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            return StatusCode(500, ApiResponse<NotificationDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("template")]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> SendTemplateNotification([FromBody] SendNotificationDto sendNotificationDto)
    {
        try
        {
            var command = new SendTemplateNotificationCommand
            {
                CustomerId = sendNotificationDto.CustomerId,
                TemplateName = sendNotificationDto.TemplateName,
                Recipient = sendNotificationDto.Recipient,
                Variables = sendNotificationDto.Variables,
                OrderId = sendNotificationDto.OrderId,
                PaymentId = sendNotificationDto.PaymentId,
                ProductId = sendNotificationDto.ProductId
            };

            var notification = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, 
                ApiResponse<NotificationDto>.SuccessResult(notification, "Template notification sent successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<NotificationDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending template notification");
            return StatusCode(500, ApiResponse<NotificationDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("templates")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationTemplateDto>>>> GetNotificationTemplates(
        [FromQuery] string? type,
        [FromQuery] bool? isActive)
    {
        try
        {
            var query = new GetNotificationTemplatesQuery
            {
                Type = type,
                IsActive = isActive
            };

            var templates = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResult(templates));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification templates");
            return StatusCode(500, ApiResponse<IEnumerable<NotificationTemplateDto>>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("templates/{name}")]
    public async Task<ActionResult<ApiResponse<NotificationTemplateDto>>> GetNotificationTemplate(string name)
    {
        try
        {
            var query = new GetNotificationTemplateByNameQuery { Name = name };
            var template = await _mediator.Send(query);

            if (template == null)
                return NotFound(ApiResponse<NotificationTemplateDto>.ErrorResult("Template not found", statusCode: 404));

            return Ok(ApiResponse<NotificationTemplateDto>.SuccessResult(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template with name: {TemplateName}", name);
            return StatusCode(500, ApiResponse<NotificationTemplateDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("templates")]
    public async Task<ActionResult<ApiResponse<NotificationTemplateDto>>> CreateNotificationTemplate([FromBody] CreateNotificationTemplateDto createTemplateDto)
    {
        try
        {
            var command = new CreateNotificationTemplateCommand
            {
                Name = createTemplateDto.Name,
                Type = createTemplateDto.Type,
                Subject = createTemplateDto.Subject,
                Content = createTemplateDto.Content,
                Description = createTemplateDto.Description,
                Variables = createTemplateDto.Variables
            };

            var template = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetNotificationTemplate), new { name = template.Name }, 
                ApiResponse<NotificationTemplateDto>.SuccessResult(template, "Template created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification template");
            return StatusCode(500, ApiResponse<NotificationTemplateDto>.ErrorResult("Internal server error"));
        }
    }
} 