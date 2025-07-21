using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.DTOs;
using PaymentService.Queries;

namespace PaymentService.Handlers;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, IEnumerable<PaymentDto>>
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;

    public GetPaymentsQueryHandler(PaymentDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Payments.AsQueryable();

        // Apply filters
        if (request.OrderId.HasValue)
            query = query.Where(p => p.OrderId == request.OrderId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(p => p.Status == request.Status);

        if (request.FromDate.HasValue)
            query = query.Where(p => p.ProcessedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(p => p.ProcessedAt <= request.ToDate.Value);

        // Apply pagination
        var payments = await query
            .OrderByDescending(p => p.ProcessedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }
}

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto?>
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;

    public GetPaymentByIdQueryHandler(PaymentDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaymentDto?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return _mapper.Map<PaymentDto>(payment);
    }
}

public class GetPaymentByNumberQueryHandler : IRequestHandler<GetPaymentByNumberQuery, PaymentDto?>
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;

    public GetPaymentByNumberQueryHandler(PaymentDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaymentDto?> Handle(GetPaymentByNumberQuery request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentNumber == request.PaymentNumber, cancellationToken);

        return _mapper.Map<PaymentDto>(payment);
    }
}

public class GetOrderPaymentsQueryHandler : IRequestHandler<GetOrderPaymentsQuery, IEnumerable<PaymentDto>>
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;

    public GetOrderPaymentsQueryHandler(PaymentDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetOrderPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _context.Payments
            .Where(p => p.OrderId == request.OrderId)
            .OrderByDescending(p => p.ProcessedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }
}

public class GetPaymentMethodsQueryHandler : IRequestHandler<GetPaymentMethodsQuery, IEnumerable<PaymentMethodDto>>
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;

    public GetPaymentMethodsQueryHandler(PaymentDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentMethodDto>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PaymentMethods.Where(pm => pm.CustomerId == request.CustomerId);

        if (request.IsActive.HasValue)
            query = query.Where(pm => pm.IsActive == request.IsActive.Value);

        var paymentMethods = await query
            .OrderByDescending(pm => pm.IsDefault)
            .ThenBy(pm => pm.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(paymentMethods);
    }
} 