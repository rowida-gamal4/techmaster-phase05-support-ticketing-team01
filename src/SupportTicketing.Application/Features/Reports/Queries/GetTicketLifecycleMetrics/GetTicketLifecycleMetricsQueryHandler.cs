using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketLifecycleMetrics;

public class GetTicketLifecycleMetricsQueryHandler
    : IRequestHandler<GetTicketLifecycleMetricsQuery, GetTicketLifecycleMetricsResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<GetTicketLifecycleMetricsQuery> validator;

    public GetTicketLifecycleMetricsQueryHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<GetTicketLifecycleMetricsQuery> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<GetTicketLifecycleMetricsResult> Handle(GetTicketLifecycleMetricsQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new UnauthorizedAccessException("Authentication is required");
        }

        var currentUserId = currentUserService.UserId.Value;

        if (currentUserService.Role != Roles.Admin && currentUserService.Role != Roles.SupportLead)
        {
            throw new ForbiddenException("Only Admin or SupportLead can view ticket lifecycle metrics");
        }

        var query = dbContext.Tickets.AsNoTracking();

        if (request.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == request.CategoryId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(t => t.CreatedAt == request.FromDate.Value);

        if (request.ToDate.HasValue)
        {
            var toDate = request.ToDate.Value.Date.AddDays(1);
            query = query.Where(t => t.CreatedAt < toDate);
        }

        var result = new GetTicketLifecycleMetricsResult
        {
            NewCount = await query.CountAsync(t => t.Status == TicketStatus.New, cancellationToken),
            InProgressCount = await query.CountAsync(t => t.Status == TicketStatus.InProgress, cancellationToken),
            ResolvedCount = await query.CountAsync(t => t.Status == TicketStatus.Resolved, cancellationToken),
            ClosedCount = await query.CountAsync(t => t.Status == TicketStatus.Closed, cancellationToken),
            CancelledCount = await query.CountAsync(t => t.Status == TicketStatus.Cancelled, cancellationToken)
        };

        result.TotalCount = result.NewCount + result.InProgressCount + result.ResolvedCount + result.ClosedCount + result.CancelledCount;

        return result;
    }
}