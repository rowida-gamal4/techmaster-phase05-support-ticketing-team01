using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.TicketAssignment;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyAgentQueue;

public class GetMyAgentQueueQueryHandler : IRequestHandler<GetMyAgentQueueQuery, GetMyAgentQueueResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<GetMyAgentQueueQuery> validator;

    public GetMyAgentQueueQueryHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<GetMyAgentQueueQuery> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<GetMyAgentQueueResult> Handle(GetMyAgentQueueQuery request, CancellationToken cancellationToken)
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

        if (currentUserService.Role != Roles.SupportAgent)
        {
            throw new ForbiddenException("Only SupportAgent can view the agent queue");
        }

        var agent = await dbContext.AgentProfiles.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == currentUserId, cancellationToken);

        if (agent is null)
            throw new NotFoundException("Agent profile not found");

        if (!agent.IsActive)
            throw new BusinessRuleException("Agent not active");

        var query = dbContext.TicketAssignments.AsNoTracking().Where(a => a.AgentId == agent.Id && a.IsActive);
        
        var orderedQuery = query.OrderByDescending(a => a.Ticket.Priority).ThenByDescending(a => a.Ticket.CreatedAt);

        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        var skip = (request.PageNumber - 1) * request.PageSize;

        var tickets = await orderedQuery.Skip(skip).Take(request.PageSize).Select(a => new AgentQueueItemDto
        {
            TicketId = a.TicketId,
            Title = a.Ticket.Title,
            Description = a.Ticket.Description,
            Status = a.Ticket.Status.ToString(),
            Priority = a.Ticket.Priority.ToString(),
            CategoryId = a.Ticket.CategoryId,
            CreatedAt = a.Ticket.CreatedAt,
            AssignedAt = a.AssignedAt
        }).ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new GetMyAgentQueueResult
        {
            Tickets = tickets,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            TotalCount = totalCount
        };

    }
}