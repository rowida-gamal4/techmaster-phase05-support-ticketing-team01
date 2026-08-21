using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Application.DTOs;

namespace SupportTicketing.Application.Features.Tickets.Commands.AssignTicket;

public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, AssignTicketResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<AssignTicketCommand> validator;

    public AssignTicketCommandHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<AssignTicketCommand> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<AssignTicketResult> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if(!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if(!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new UnauthorizedAccessException("Authentication is required");
        }

        var currentUserId = currentUserService.UserId.Value;

        if (currentUserService.Role != Roles.Admin && currentUserService.Role != Roles.SupportLead)
        {
            throw new ForbiddenException("Only Admin or SupportLead can assign tickets");
        }

        var ticket = await dbContext.Tickets.Include(t => t.Assignments).FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
            throw new NotFoundException("Ticket not found");

        if(ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Cancelled)
            throw new BusinessRuleException("A closed or cancelled ticket cannot be assigned");

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

        if (activeAssignment is not null)
            throw new BusinessRuleException("Ticket is already assigned, use reassign operation instead");

        var assignment = new TicketAssignment
        {
            TicketId = ticket.Id,
            AgentId = agent.Id,
            TeamId = team.Id,
            AssignedByUserId = currentUserId,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };
        await dbContext.TicketAssignments.AddAsync(assignment);

        ticket.Status = TicketStatus.Assigned;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AssignTicketResult
        {
            TicketId = ticket.Id,
            AgentId = agent.Id,
            TeamId = team.Id,
            AssignedAt = assignment.AssignedAt,
            Status = ticket.Status.ToString()
        };
    }
}