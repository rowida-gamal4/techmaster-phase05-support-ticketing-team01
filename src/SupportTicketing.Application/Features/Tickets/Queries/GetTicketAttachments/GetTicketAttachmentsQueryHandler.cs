using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetTicketAttachments
{
    public class GetTicketAttachmentsQueryHandler : IRequestHandler<GetTicketAttachmentsQuery, GetTicketAttachmentsResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly IValidator<GetTicketAttachmentsQuery> validator;

        public GetTicketAttachmentsQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<GetTicketAttachmentsQuery> validator)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.validator = validator;
        }

        public async Task<GetTicketAttachmentsResult> Handle(GetTicketAttachmentsQuery request, CancellationToken cancellationToken)
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

            var attachments = await dbContext.TicketAttachments.Include(a => a.Ticket).AsNoTracking().ToListAsync(cancellationToken);

            if (request.Request.TicketId.HasValue)
            {
                var ticketExist = await dbContext.Tickets.AnyAsync(t => t.Id == request.Request.TicketId.Value, cancellationToken);

                if (!ticketExist)
                {
                    throw new NotFoundException("Ticket not found.");
                }

                attachments = attachments.Where(a => a.TicketId == request.Request.TicketId.Value).ToList();
            }

            if (role == Roles.Admin)
            {

            }
            else if (role == Roles.Customer)
            {
                var customer = await dbContext.CustomerProfiles.FirstOrDefaultAsync(c => c.UserId == currentUserId, cancellationToken);

                if (customer is null)
                {
                    throw new ForbiddenException("Customer profile not found.");
                }

                attachments = attachments.Where(a => a.Ticket.CustomerId == customer.Id).ToList();
            }
            else if (role == Roles.SupportAgent)
            {
                var agent = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == currentUserId, cancellationToken);

                if (agent is null)
                {
                    throw new ForbiddenException("Agent profile not found.");
                }

                var ticketIds = await dbContext.TicketAssignments.Where(a => a.AgentId == agent.Id && a.IsActive).Select(a => a.TicketId).ToListAsync(cancellationToken);

                attachments = attachments.Where(a => ticketIds.Contains(a.TicketId)).ToList();
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

                var ticketIds = await dbContext.TicketAssignments.Where(a => a.TeamId == lead.SupportTeamId && a.IsActive).Select(a => a.TicketId).ToListAsync(cancellationToken);

                attachments = attachments.Where(a => ticketIds.Contains(a.TicketId)).ToList();
            }
            else
            {
                throw new ForbiddenException("You are not allowed to see attachments of tickets.");
            }
            var result = attachments.Select(a => new TicketAttachmentMetadataResponseDto
            {
                Id = a.Id,
                TicketId = a.TicketId,
                UploadedByUserId = a.UploadedByUserId,
                FileName = a.FileName,
                FileSize = a.FileSize,
                ContentType = a.ContentType,
                StorageKey = a.StorageKey,
                CreatedAt = a.CreatedAt
            }).ToList();

            return new GetTicketAttachmentsResult
            {
                Attachments = result
            };
        }
    }
}