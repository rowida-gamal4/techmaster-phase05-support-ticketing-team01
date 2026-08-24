using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Reports.Queries.GetHighPriorityOpenTickets
{
    public class GetHighPriorityOpenTicketsQueryHandler : IRequestHandler<GetHighPriorityOpenTicketsQuery, GetHighPriorityOpenTicketsResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;

        public GetHighPriorityOpenTicketsQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
        }
        public async Task<GetHighPriorityOpenTicketsResult> Handle(GetHighPriorityOpenTicketsQuery request, CancellationToken cancellationToken)
        {
            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required.");
            }

            var UserId = currentUserService.UserId.Value;
            var role = currentUserService.Role;

            if (role != Roles.Admin && role != Roles.SupportLead)
            {
                throw new ForbiddenException("Only Admin and SupportLead can view high priority tickets.");
            }

            if (role == Roles.SupportLead)
            {
                var lead = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == UserId, cancellationToken);

                if (lead is null)
                {
                    throw new ForbiddenException("SupportLead profile not found.");
                }
            }
            if (request.Request.PageNumber < 1)
            {
                throw new BusinessRuleException("Page number must be greater than 0.");
            }

            if (request.Request.PageSize < 1 || request.Request.PageSize > 100)
            {
                throw new BusinessRuleException("Page size must be between 1 and 100.");
            }

            var query = dbContext.Tickets.AsNoTracking().Where(t => (t.Priority == TicketPriority.High || t.Priority == TicketPriority.Critical) &&
                t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed && t.Status != TicketStatus.Cancelled);

            var totalCount = await query.CountAsync(cancellationToken);

            var tickets = await query.Skip((request.Request.PageNumber - 1) * request.Request.PageSize).Take(request.Request.PageSize).Select(t => new HighPriorityTicketDto
            {
                TicketId = t.Id,
                Title = t.Title,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CategoryId = t.CategoryId,
                CreatedAt = t.CreatedAt
            }).ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.Request.PageSize);

            return new GetHighPriorityOpenTicketsResult
            {
                Tickets = tickets,
                PageNumber = request.Request.PageNumber,
                PageSize = request.Request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}