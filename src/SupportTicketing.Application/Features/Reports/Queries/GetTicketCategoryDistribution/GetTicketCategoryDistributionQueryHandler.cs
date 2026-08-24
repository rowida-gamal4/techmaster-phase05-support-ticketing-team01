using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketCategoryDistribution
{
    public class GetTicketCategoryDistributionQueryHandler : IRequestHandler<GetTicketCategoryDistributionQuery, GetTicketCategoryDistributionResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        public GetTicketCategoryDistributionQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
        }

        public async Task<GetTicketCategoryDistributionResult> Handle(GetTicketCategoryDistributionQuery request, CancellationToken cancellationToken)
        {
            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required.");
            }

            var UserId = currentUserService.UserId.Value;
            var role = currentUserService.Role;

            if (role != Roles.Admin && role != Roles.SupportLead)
            {
                throw new ForbiddenException("Only Admin and SupportLead can view ticket category distribution.");
            }

            if (role == Roles.SupportLead)
            {
                var lead = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == UserId, cancellationToken);

                if (lead is null)
                {
                    throw new ForbiddenException("SupportLead profile not found.");
                }
            }

            var categories = await dbContext.TicketCategories.AsNoTracking().Select(c => new TicketCategoryDistributionDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                CategoryCode = c.Code,
                TicketCount = c.Tickets.Count()
            }).OrderByDescending(x => x.TicketCount).ToListAsync(cancellationToken);

            return new GetTicketCategoryDistributionResult
            {
                Categories = categories
            };

        }

    }
}