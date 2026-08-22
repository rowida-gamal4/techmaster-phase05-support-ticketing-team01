using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.SetTicketPriority;

public class SetPriorityCommandHandler : IRequestHandler<SetPriorityCommand, SetPriorityResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<SetPriorityCommand> validator;

    public SetPriorityCommandHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<SetPriorityCommand> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<SetPriorityResult> Handle(SetPriorityCommand request, CancellationToken cancellationToken)
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
            throw new ForbiddenException("Only Admin or SupportLead can change ticket priority");
        }

        var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
            throw new NotFoundException("Ticket not found");

        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Cancelled)
            throw new BusinessRuleException("A closed or cancelled ticket cannot have its priority changed");

        var oldPriority = ticket.Priority;

        ticket.Priority = (TicketPriority)request.Request.Priority;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new SetPriorityResult
        {
            TicketId = ticket.Id,
            Priority = ticket.Priority.ToString(),
            Status = ticket.Status.ToString()
        };

    }
}