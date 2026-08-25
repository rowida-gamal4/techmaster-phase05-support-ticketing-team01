using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.ResolveTicket;

public class ResolveTicketCommandHandler : IRequestHandler<ResolveTicketCommand, ResolveTicketResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<ResolveTicketCommand> validator;

    public ResolveTicketCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<ResolveTicketCommand> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<ResolveTicketResult> Handle(ResolveTicketCommand request, CancellationToken cancellationToken)
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
            throw new ForbiddenException("Only SupportAgent can resolve ticket ");
        }

        var agent = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == currentUserId, cancellationToken);

        if (agent is null)
            throw new NotFoundException("Agent profile not found");

        if (!agent.IsActive)
            throw new BusinessRuleException("Agent not active");


        var ticket = await dbContext.Tickets.Include(t => t.Assignments).FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
            throw new NotFoundException("Ticket not found");

        if (ticket.Status != TicketStatus.InProgress)
            throw new BusinessRuleException($"Ticket cannot be resolved from {ticket.Status} status");

        var activeAssignment = ticket.Assignments.FirstOrDefault(a => a.IsActive);

        if (activeAssignment is null)
            throw new BusinessRuleException("Ticket is not currently assigned to an agent");

        //varify ownership
        if (activeAssignment.AgentId != agent.Id)
            throw new ForbiddenException("You can only resolve tickets assigned to you");

        var oldStatus = ticket.Status;

        ticket.Status = TicketStatus.Resolved;
        ticket.ResolutionNotes = request.Request.ResolutionNotes.Trim();
        ticket.ResolvedAt = DateTime.UtcNow;

        var activityLog = new ActivityLog
        {
            UserId = currentUserId,
            EntityName = nameof(Ticket),
            EntityId = ticket.Id,
            Action = "Ticket resolved"
        };

        await dbContext.ActivityLogs.AddAsync(activityLog, cancellationToken);

        var statusHistory = new TicketStatusHistory
        {
            TicketId = ticket.Id,
            ChangedByUserId = currentUserId,
            ChangedAt = DateTime.UtcNow,
            OldStatus = oldStatus,
            NewStatus = TicketStatus.Resolved,
            Reason = request.Request.ResolutionNotes
        };

        await dbContext.TicketStatusHistories.AddAsync(statusHistory, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ResolveTicketResult
        {
            TicketId = ticket.Id,
            Status = ticket.Status.ToString(),
            ResolutionNotes = ticket.ResolutionNotes,
            ResolvedAt = ticket.ResolvedAt.Value
        };
    }
}