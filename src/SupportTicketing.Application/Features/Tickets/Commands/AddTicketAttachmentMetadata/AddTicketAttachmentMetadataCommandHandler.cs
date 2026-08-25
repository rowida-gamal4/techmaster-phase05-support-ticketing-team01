using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.AddTicketAttachmentMetadata
{
    public class AddTicketAttachmentMetadataCommandHandler : IRequestHandler<AddTicketAttachmentMetadataCommand, AddTicketAttachmentMetadataResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly IValidator<AddTicketAttachmentMetadataCommand> validator;

        public AddTicketAttachmentMetadataCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<AddTicketAttachmentMetadataCommand> validator)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.validator = validator;
        }
        public async Task<AddTicketAttachmentMetadataResult> Handle(AddTicketAttachmentMetadataCommand request, CancellationToken cancellationToken)
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

            var currentUserId = currentUserService.UserId.Value;
            var role = currentUserService.Role;

            var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

            if (ticket is null)
            {
                throw new NotFoundException("Ticket not found.");
            }

            if (role == Roles.Customer)
            {
                var customer = await dbContext.CustomerProfiles.FirstOrDefaultAsync(c => c.UserId == currentUserId, cancellationToken);

                if (customer is null)
                {
                    throw new ForbiddenException("Customer profile not found.");
                }

                if (ticket.CustomerId != customer.Id)
                {
                    throw new ForbiddenException("This ticket does not belong to you.");
                }
            }
            else if (role == Roles.SupportAgent)
            {
                var agent = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == currentUserId, cancellationToken);
                if (agent is null)
                {
                    throw new ForbiddenException("Agent profile not found.");
                }

                var isAssigned = await dbContext.TicketAssignments.AnyAsync(a => a.TicketId == ticket.Id && a.Agent.UserId == currentUserId && a.IsActive, cancellationToken);
                if (!isAssigned)
                {
                    throw new ForbiddenException("You are not assigned to this ticket.");
                }
            }
            else if (role == Roles.SupportLead)
            {
                var lead = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == currentUserId, cancellationToken);

                if (lead is null)
                {
                    throw new ForbiddenException("SupportLead Profile not found.");
                }

                if (lead.SupportTeamId is null)
                {
                    throw new ForbiddenException("SupportLead is not assigned to a team yet.");
                }

                var belongs = await dbContext.TicketAssignments.AnyAsync(a => a.TicketId == ticket.Id && a.TeamId == lead.SupportTeamId, cancellationToken);
                if (!belongs)
                {
                    throw new ForbiddenException("This ticket does not belong to your team.");
                }
            }
            else
            {
                throw new ForbiddenException("You are not allowed to add attachments to this ticket.");
            }

            var extension = Path.GetExtension(request.Request.FileName);
            var storageKey = $"tickets/{ticket.Id}/{Guid.NewGuid()}{extension}";

            var attachment = new TicketAttachmentMetadata
            {
                TicketId = ticket.Id,
                UploadedByUserId = currentUserId,
                FileName = request.Request.FileName,
                FileSize = request.Request.FileSize,
                ContentType = request.Request.ContentType,
                StorageKey = storageKey
            };
            await dbContext.TicketAttachments.AddAsync(attachment, cancellationToken);

            var activityLog = new ActivityLog
            {
                UserId = currentUserId,
                EntityName = nameof(Ticket),
                EntityId = ticket.Id,
                Action = $"Attachment metadata added: {attachment.FileName}"
            };

            await dbContext.ActivityLogs.AddAsync(activityLog, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            return new AddTicketAttachmentMetadataResult
            {
                Attachment = new TicketAttachmentMetadataResponseDto
                {
                    Id = attachment.Id,
                    TicketId = attachment.TicketId,
                    UploadedByUserId = attachment.UploadedByUserId,
                    FileName = attachment.FileName,
                    FileSize = attachment.FileSize,
                    ContentType = attachment.ContentType,
                    StorageKey = attachment.StorageKey,
                    CreatedAt = attachment.CreatedAt
                }
            };
        }
    }
}