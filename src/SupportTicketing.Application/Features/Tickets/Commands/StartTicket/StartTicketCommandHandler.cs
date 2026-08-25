using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.StartTicket;

public class StartTicketCommandHandler : IRequestHandler<StartTicketCommand, StartTicketResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<StartTicketCommand> validator;

    public StartTicketCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<StartTicketCommand> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<StartTicketResult> Handle(StartTicketCommand request, CancellationToken cancellationToken)
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
            throw new ForbiddenException("Only SupportAgent can move assigned ticket to InProgress ");
        }

        var agent = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == currentUserId, cancellationToken);

        if (agent is null)
            throw new NotFoundException("Agent profile not found");

        if (!agent.IsActive)
            throw new BusinessRuleException("Agent not active");


        var ticket = await dbContext.Tickets.Include(t => t.Assignments).FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
            throw new NotFoundException("Ticket not found");

        if (ticket.Status != TicketStatus.Assigned)
            throw new BusinessRuleException($"Ticket cannot be moved to InProgress fron {ticket.Status} status");

        var activeAssignment = ticket.Assignments.FirstOrDefault(a => a.IsActive);

        if (activeAssignment is null)
            throw new BusinessRuleException("Ticket is not currently assigned to an agent");

        //varify ownership
        if (activeAssignment.AgentId != agent.Id)
            throw new ForbiddenException("You can only start tickets assigned to you");

        var oldStatus = ticket.Status;

        ticket.Status = TicketStatus.InProgress;

        if (ticket.StartedAt is null)
            ticket.StartedAt = DateTime.UtcNow;

        var statusHistory = new TicketStatusHistory
        {
            TicketId = ticket.Id,
            ChangedByUserId = currentUserId,
            ChangedAt = DateTime.UtcNow,
            OldStatus = oldStatus,
            NewStatus = TicketStatus.InProgress,
            Reason = "Ticket work started by assigned agent"
        };

        await dbContext.TicketStatusHistories.AddAsync(statusHistory, cancellationToken);

        var activityLog = new ActivityLog
        {
            UserId = currentUserId,
            EntityName = nameof(Ticket),
            EntityId = ticket.Id,
            Action = "TicketStarted"
        };

        await dbContext.ActivityLogs.AddAsync(activityLog, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new StartTicketResult
        {
            TicketId = ticket.Id,
            Status = ticket.Status.ToString(),
            StartedAt = ticket.StartedAt.Value
        };
    }
}