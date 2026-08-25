using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Agent;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Agent.Queries.GetMyActiveTickets;

public class GetMyActiveTicketsQueryHandler
    : IRequestHandler<GetMyActiveTicketsQuery, GetMyActiveTicketsResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;

    public GetMyActiveTicketsQueryHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
    }
    public async Task<GetMyActiveTicketsResult> Handle(GetMyActiveTicketsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new UnauthorizedAccessException("Authentication is required");
        }

        if (currentUserService.Role != Roles.SupportAgent)
        {
            throw new ForbiddenException("Only SupportAgent can view active tickets");
        }

        var currentUserId = currentUserService.UserId.Value;

        var agent = await dbContext.AgentProfiles.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == currentUserId, cancellationToken);

        if (agent == null)
            throw new NotFoundException("Agent profile not found");

        if(!agent.IsActive)
            throw new BusinessRuleException("Agent is not active");

        var query = dbContext.Tickets.AsNoTracking().Where(t => t.Assignments.Any(a => a.AgentId == agent.Id && a.IsActive) &&
            t.Status != TicketStatus.Resolved &&
            t.Status != TicketStatus.Closed &&
            t.Status != TicketStatus.Cancelled);

        if (request.SortBy.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
            query = query.OrderByDescending(a => a.CreatedAt);

        else if (request.SortBy.Equals("priority", StringComparison.OrdinalIgnoreCase))
            query = query.OrderByDescending(t => t.Priority).ThenByDescending(t => t.CreatedAt);

        else
            throw new BusinessRuleException("SortBy must be 'priority' or 'createdAt'");


        var ticket = await query
            .Select(t => new MyActiveTicketDto
            {
                TicketId = t.Id,
                Title = t.Title,
                CategoryId = t.CategoryId,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                CreatedAt = t.CreatedAt,
                StartedAt = t.StartedAt,
            }).ToListAsync(cancellationToken);

        
        return new GetMyActiveTicketsResult
        {
            Tickets = ticket
        };

    }
}