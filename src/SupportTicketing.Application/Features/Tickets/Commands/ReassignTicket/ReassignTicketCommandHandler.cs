using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.ReassignTicket;

public class ReassignTicketCommandHandler : IRequestHandler<ReassignTicketCommand, ReassignTicketResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<ReassignTicketCommand> validator;

    public ReassignTicketCommandHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<ReassignTicketCommand> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<ReassignTicketResult> Handle(ReassignTicketCommand request, CancellationToken cancellationToken)
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

        if (currentUserService.Role != Roles.SupportLead)
        {
            throw new ForbiddenException("Only SupportLead can assign tickets");
        }

        var ticket = await dbContext.Tickets.Include(t => t.Assignments).FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
            throw new NotFoundException("Ticket not found");

        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Cancelled)
            throw new BusinessRuleException("A closed or cancelled ticket cannot be reassigned");

        var agent = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.Id == request.Request.AgentId, cancellationToken);

        if (agent is null)
            throw new NotFoundException("Agent profile not found");

        if (!agent.IsActive)
            throw new BusinessRuleException("Agent not active");

        var team = await dbContext.SupportTeams.FirstOrDefaultAsync(t => t.Id == request.Request.TeamId, cancellationToken);

        if (team is null)
            throw new NotFoundException("Support team not found");

        if (!team.IsActive)
            throw new BusinessRuleException("Support team not active");

        var activeAssignment = ticket.Assignments.FirstOrDefault(a => a.IsActive);

        if (activeAssignment is null)
            throw new BusinessRuleException("Ticket does not have active assignment, Use assign operation first");

        if (activeAssignment.AgentId == agent.Id && activeAssignment.TeamId == team.Id)
            throw new BusinessRuleException("Ticket is already assigned to this agent and team");

        var oldAgentId = activeAssignment.AgentId;

        activeAssignment.IsActive = false;
        activeAssignment.EndedAt = DateTime.UtcNow;

        var reassignedAt = DateTime.UtcNow;

        var newAssignment = new TicketAssignment
        {
            TicketId = ticket.Id,
            AgentId = agent.Id,
            TeamId = team.Id,
            AssignedByUserId = currentUserId,
            AssignedAt = reassignedAt,
            IsActive = true
        };
        await dbContext.TicketAssignments.AddAsync(newAssignment,cancellationToken);

        ticket.Status = TicketStatus.Assigned;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ReassignTicketResult
        {
            TicketId = ticket.Id,
            OldAgentId = oldAgentId,
            NewAgentId = agent.Id,
            NewTeamId = team.Id,
            ReassignedAt = reassignedAt,
            Status = ticket.Status.ToString()
        };
    }
}