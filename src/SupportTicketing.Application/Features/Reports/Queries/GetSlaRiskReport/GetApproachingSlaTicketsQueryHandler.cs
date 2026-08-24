using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Sla;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Sla.Queries.GetSlaRiskReport;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetSlaRiskReport;

public class GetApproachingSlaTicketsQueryHandler : IRequestHandler<GetApproachingSlaTicketsQuery, GetApproachingSlaTicketsResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;

    public GetApproachingSlaTicketsQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
    }

    public async Task<GetApproachingSlaTicketsResult> Handle(GetApproachingSlaTicketsQuery request, CancellationToken cancellationToken)
    {

        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new UnauthorizedAccessException("Authentication is required.");
        }

        if (currentUserService.Role != Roles.Admin && currentUserService.Role != Roles.SupportLead)
        {
            throw new ForbiddenException("Only Admin or SupportLead can view SLA risk tickets.");
        }

        var now = DateTime.UtcNow;
        var threshold = now.AddHours(48);
        var result = new List<SlaTicketResponseDto>();

        var tickets = await dbContext.Tickets.Where(t => t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed && t.Status != TicketStatus.Cancelled).ToListAsync(cancellationToken);

        var policies = await dbContext.SlaPolicies.Where(p => p.IsActive).ToListAsync(cancellationToken);

        foreach (var ticket in tickets)
        {
            var policy = policies.FirstOrDefault(p => p.CategoryId == ticket.CategoryId && p.Priority == ticket.Priority);

            if (policy is null)
            {
                throw new BusinessRuleException("No active SLA policy exists for this category and priority.");
            }

            var targetTime = ticket.CreatedAt.AddMinutes(policy.ResolutionTimeMin);

            if (targetTime <= threshold)
            {
                var timeRemaining = targetTime - now;
                var isBreached = targetTime < now;
                var minutesRemaining = isBreached ? 0 : (int)Math.Ceiling((targetTime - now).TotalMinutes);

                result.Add(new SlaTicketResponseDto
                {
                    TicketId = ticket.Id,
                    Title = ticket.Title,
                    Status = ticket.Status.ToString(),
                    Priority = ticket.Priority.ToString(),
                    CategoryId = ticket.CategoryId,
                    CreatedAt = ticket.CreatedAt,
                    SlaTargetAt = targetTime,
                    IsBreached = isBreached,
                    MinutesRemaining = minutesRemaining
                });
            }
        }


        return new GetApproachingSlaTicketsResult
        {
            Tickets = result.OrderBy(t => t.SlaTargetAt).ToList()
        };
    }
}