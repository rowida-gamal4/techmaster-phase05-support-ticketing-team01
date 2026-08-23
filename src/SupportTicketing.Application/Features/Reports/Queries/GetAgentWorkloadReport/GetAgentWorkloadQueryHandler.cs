using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Reports.Queries.GetAgentWorkloadReport
{
    public class GetAgentWorkloadQueryHandler : IRequestHandler<GetAgentWorkloadQuery, GetAgentWorkloadResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public GetAgentWorkloadQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext ,UserManager<ApplicationUser> userManager)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.userManager = userManager ;
        }
        public async Task<GetAgentWorkloadResult> Handle(GetAgentWorkloadQuery request, CancellationToken cancellationToken)
        {
            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required.");
            }

            var UserId = currentUserService.UserId.Value;
            var userRole = currentUserService.Role;

            if (userRole != Roles.SupportLead)
            {
                throw new ForbiddenException("Only SupportLead allowed to view agents workload.");
            }

            var lead = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == UserId, cancellationToken);

            if (lead is null)
            {
                throw new ForbiddenException("Support lead profile was not found.");
            }
            var supportAgents = await userManager.GetUsersInRoleAsync(Roles.SupportAgent);

            var supportAgentUserIds = supportAgents.Select(u => u.Id).ToList();

            var agents = await dbContext.AgentProfiles.Where(a => a.SupportTeamId == lead.SupportTeamId &&  supportAgentUserIds.Contains(a.UserId)).Select(agent => new AgentWorkloadResponseDto
            {
                AgentId = agent.Id,
                AgentName = agent.FullName,
                TotalAssignedTickets = agent.Assignments.Count(),
                ActiveTickets = agent.Assignments.Count(a => a.Ticket.Status != TicketStatus.Resolved && a.Ticket.Status != TicketStatus.Closed && a.Ticket.Status != TicketStatus.Cancelled)
            }).ToListAsync(cancellationToken);

            return new GetAgentWorkloadResult
            {
                Agents = agents
            };
        }
    }
}