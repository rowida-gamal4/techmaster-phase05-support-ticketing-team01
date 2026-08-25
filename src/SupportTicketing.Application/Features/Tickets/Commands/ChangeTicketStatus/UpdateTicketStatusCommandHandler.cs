using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.ChangeTicketStatus;

public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, UpdateTicketStatusResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<UpdateTicketStatusCommand> validator;

    public UpdateTicketStatusCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<UpdateTicketStatusCommand> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }

    public async Task<UpdateTicketStatusResult> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }


        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new UnauthorizedAccessException("Authentication is required.");
        }

        var userId = currentUserService.UserId.Value;
        var role = currentUserService.Role;


        if (!Enum.TryParse<TicketStatus>(request.Request.Status, true, out var newStatus))
        {
            throw new ArgumentException("Invalid ticket status.");
        }


        var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found.");
        }

        var oldStatus = ticket.Status;


        if (role == Roles.Customer)
        {
            var customer = await dbContext.CustomerProfiles.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (customer is null)
            {
                throw new ForbiddenException("Customer profile not found.");
            }

            if (ticket.CustomerId != customer.Id)
            {
                throw new ForbiddenException("This ticket does not belong to you.");
            }

            if (newStatus != TicketStatus.Closed)
            {
                throw new ForbiddenException("Customers can only change status to close.");
            }

            if (ticket.Status != TicketStatus.Resolved)
            {
                throw new BusinessRuleException("Only resolved tickets can be closed.");
            }

            ticket.Status = TicketStatus.Closed;
            ticket.ClosedAt = DateTime.UtcNow;
        }
        else if (role == Roles.SupportLead)
        {

            if (newStatus == TicketStatus.Closed)
            {
                if (ticket.Status != TicketStatus.Resolved)
                {
                    throw new BusinessRuleException("Only resolved tickets can be closed.");
                }

                ticket.Status = TicketStatus.Closed;
                ticket.ClosedAt = DateTime.UtcNow;
            }

            else if (newStatus == TicketStatus.Reopened)
            {
                if (ticket.Status != TicketStatus.Closed)
                {
                    throw new BusinessRuleException("Only closed tickets can be reopened.");
                }

                ticket.Status = TicketStatus.Reopened;
            }
            else
            {
                throw new BusinessRuleException("supports only to close or reopen tickets.");
            }
        }
        else
        {
            throw new ForbiddenException("You are not allowed to change this ticket status.");
        }


        var history = new TicketStatusHistory
        {
            TicketId = ticket.Id,
            OldStatus = oldStatus,
            NewStatus = ticket.Status,
            ChangedByUserId = userId,
            ChangedAt = DateTime.UtcNow,
            Reason = "Ticket status changed"
        };

        await dbContext.TicketStatusHistories.AddAsync(history);

        var activityLog = new ActivityLog
        {
            UserId = userId,
            EntityName = nameof(Ticket),
            EntityId = ticket.Id,
            Action = $"Status changed from {oldStatus} to {ticket.Status}"
        };

        await dbContext.ActivityLogs.AddAsync(activityLog, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateTicketStatusResult
        {
            Ticket = new TicketStatusResponseDto
            {
                TicketId = ticket.Id,
                Status = ticket.Status.ToString(),
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt
            }
        };
    }
}