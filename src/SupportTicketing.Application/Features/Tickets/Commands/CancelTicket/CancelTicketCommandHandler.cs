using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.CancelTicket
{
    public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand, CancelTicketResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly IValidator<CancelTicketCommand> validator;

        public CancelTicketCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<CancelTicketCommand> validator)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.validator = validator;
        }
        public async Task<CancelTicketResult> Handle(CancelTicketCommand request, CancellationToken cancellationToken)
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

            var UserId = currentUserService.UserId.Value;

            var customer = await dbContext.CustomerProfiles.FirstOrDefaultAsync(c => c.UserId == UserId, cancellationToken);

            if (customer is null)
            {
                throw new NotFoundException("Customer profile was not found.");
            }

            var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CustomerId == customer.Id, cancellationToken);

            if (ticket is null)
            {
                throw new NotFoundException("Ticket was not found.");
            }

            if (ticket.Status != TicketStatus.New)
            {
                throw new BusinessRuleException("Only new tickets can be cancelled.");
            }

            ticket.Status = TicketStatus.Cancelled;
            ticket.CancellationReason = request.Request.CancellationReason;
            ticket.CancelledAt = DateTime.UtcNow;

            var history = new TicketStatusHistory
            {
                TicketId = ticket.Id,
                OldStatus = TicketStatus.New,
                NewStatus = TicketStatus.Cancelled,
                Reason = request.Request.CancellationReason,
                ChangedByUserId = UserId

            };
            dbContext.TicketStatusHistories.Add(history);

            await dbContext.SaveChangesAsync(cancellationToken);

            var activityLog = new ActivityLog
            {
                UserId = UserId,
                EntityName = nameof(Ticket),
                EntityId = ticket.Id,
                Action = "Cancell Ticket"
            };

            await dbContext.ActivityLogs.AddAsync(activityLog,cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return new CancelTicketResult
            {
                Ticket = new CancelTicketResponseDTo
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Priority = ticket.Priority.ToString(),
                    CreatedAt = ticket.CreatedAt,
                    CancelledAt = ticket.CancelledAt,
                    CancellationReason = request.Request.CancellationReason
                }
            };


        }
    }
}