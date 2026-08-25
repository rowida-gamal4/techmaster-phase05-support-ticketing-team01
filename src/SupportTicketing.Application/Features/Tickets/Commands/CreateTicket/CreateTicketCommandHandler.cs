using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Common.Models;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, CreateTicketResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly IValidator<CreateTicketCommand> validator;

        public CreateTicketCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<CreateTicketCommand> validator)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.validator = validator;
        }
        public async Task<CreateTicketResult> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
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

            var category = await dbContext.TicketCategories.FirstOrDefaultAsync(
            c => c.Id == request.Request.CategoryId, cancellationToken);

            if (category is null)
            {
                throw new NotFoundException("Ticket category was not found.");
            }

            var ticket = new Ticket
            {
                CustomerId = customer.Id,
                CategoryId = request.Request.CategoryId,
                Title = request.Request.Title,
                Description = request.Request.Description,
                Status = TicketStatus.New,
                Priority = TicketPriority.Low
            };
            ticket.SetCreatedAt();
            dbContext.Tickets.Add(ticket);

            await dbContext.SaveChangesAsync(cancellationToken);

            var activityLog = new ActivityLog
            {
                UserId = UserId,
                EntityName = nameof(Ticket),
                EntityId = ticket.Id,
                Action = "Ticket Created"
            };

            await dbContext.ActivityLogs.AddAsync(activityLog, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateTicketResult
            {
                Ticket = new TicketResponseDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    CategoryId = ticket.CategoryId,
                    Status = ticket.Status.ToString(),
                    Priority = ticket.Priority.ToString(),
                    CreatedAt = ticket.CreatedAt
                }
            };


        }
    }
}