using AutoMapper;
using ECommerce.Events;
using ECommerce.Messaging.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationService.Commands;
using NotificationService.Data;
using NotificationService.DTOs;
using NotificationService.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationService.Handlers;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, NotificationDto>
{
    private readonly NotificationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<SendNotificationCommandHandler> _logger;
    private readonly IConfiguration _configuration;

    public SendNotificationCommandHandler(
        NotificationDbContext context,
        IMapper mapper,
        IMessageProducer messageProducer,
        ILogger<SendNotificationCommandHandler> logger,
        IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _messageProducer = messageProducer;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<NotificationDto> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        // Generate notification number
        var notificationNumber = GenerateNotificationNumber();
        
        var notification = new Notification
        {
            CustomerId = request.CustomerId,
            NotificationNumber = notificationNumber,
            Type = request.Type,
            Subject = request.Subject,
            Content = request.Content,
            Recipient = request.Recipient,
            Status = "Pending",
            OrderId = request.OrderId,
            PaymentId = request.PaymentId,
            ProductId = request.ProductId,
            Metadata = request.Metadata
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        // Send notification based on type
        try
        {
            switch (request.Type.ToLower())
            {
                case "email":
                    await SendEmailAsync(notification);
                    break;
                case "sms":
                    await SendSmsAsync(notification);
                    break;
                default:
                    throw new ArgumentException($"Unsupported notification type: {request.Type}");
            }

            notification.Status = "Sent";
            notification.SentAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            notification.Status = "Failed";
            notification.FailureReason = ex.Message;
            _logger.LogError(ex, "Failed to send notification {NotificationId}", notification.Id);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Notification sent with ID: {NotificationId}, Type: {Type}", notification.Id, notification.Type);

        return _mapper.Map<NotificationDto>(notification);
    }

    private async Task SendEmailAsync(Notification notification)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var fromEmail = _configuration["SendGrid:FromEmail"];
        var fromName = _configuration["SendGrid:FromName"];

        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("SendGrid API key not configured");

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(notification.Recipient);
        var msg = MailHelper.CreateSingleEmail(from, to, notification.Subject, notification.Content, notification.Content);
        
        var response = await client.SendEmailAsync(msg);
        
        if (response.IsSuccessStatusCode)
        {
            notification.ExternalId = response.Headers.GetValues("X-Message-Id").FirstOrDefault();
        }
        else
        {
            throw new Exception($"SendGrid API error: {response.StatusCode}");
        }
    }

    private async Task SendSmsAsync(Notification notification)
    {
        var accountSid = _configuration["Twilio:AccountSid"];
        var authToken = _configuration["Twilio:AuthToken"];
        var fromPhoneNumber = _configuration["Twilio:FromPhoneNumber"];

        if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
            throw new InvalidOperationException("Twilio credentials not configured");

        TwilioClient.Init(accountSid, authToken);

        var message = await MessageResource.CreateAsync(
            body: notification.Content,
            from: new PhoneNumber(fromPhoneNumber),
            to: new PhoneNumber(notification.Recipient)
        );

        notification.ExternalId = message.Sid;
    }

    private string GenerateNotificationNumber()
    {
        return $"NOT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
}

public class SendTemplateNotificationCommandHandler : IRequestHandler<SendTemplateNotificationCommand, NotificationDto>
{
    private readonly NotificationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<SendTemplateNotificationCommandHandler> _logger;

    public SendTemplateNotificationCommandHandler(
        NotificationDbContext context,
        IMapper mapper,
        ILogger<SendTemplateNotificationCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<NotificationDto> Handle(SendTemplateNotificationCommand request, CancellationToken cancellationToken)
    {
        // Get template
        var template = await _context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Name == request.TemplateName && t.IsActive, cancellationToken);

        if (template == null)
            throw new ArgumentException($"Template '{request.TemplateName}' not found or inactive");

        // Replace variables in template
        var subject = ReplaceVariables(template.Subject, request.Variables);
        var content = ReplaceVariables(template.Content, request.Variables);

        // Create and send notification
        var notification = new Notification
        {
            CustomerId = request.CustomerId,
            NotificationNumber = $"NOT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
            Type = template.Type,
            Subject = subject,
            Content = content,
            Recipient = request.Recipient,
            Status = "Pending",
            OrderId = request.OrderId,
            PaymentId = request.PaymentId,
            ProductId = request.ProductId
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Template notification sent with ID: {NotificationId}, Template: {TemplateName}", 
            notification.Id, request.TemplateName);

        return _mapper.Map<NotificationDto>(notification);
    }

    private string ReplaceVariables(string template, Dictionary<string, string>? variables)
    {
        if (variables == null) return template;

        var result = template;
        foreach (var variable in variables)
        {
            result = result.Replace($"{{{variable.Key}}}", variable.Value);
        }
        return result;
    }
} 