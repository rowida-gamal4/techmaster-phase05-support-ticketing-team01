using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Agent;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Agent.Queries.GetMyTeamQueue
{
    public class GetMyTeamQueueQueryHandler : IRequestHandler<GetMyTeamQueueQuery, GetMyTeamQueueResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;

        public GetMyTeamQueueQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
        }
        public async Task<GetMyTeamQueueResult> Handle(GetMyTeamQueueQuery request, CancellationToken cancellationToken)
        {
            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required.");
            }

            var UserId = currentUserService.UserId.Value;
            var role = currentUserService.Role;

            if (role != Roles.SupportLead)
            {
                throw new ForbiddenException("Only SupportLead can view the team queue.");
            }

            var lead = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == UserId, cancellationToken);

            if (lead is null)
            {
                throw new ForbiddenException("SupportLead profile not found.");
            }

            if (lead.SupportTeamId is null)
            {
                throw new BusinessRuleException("SupportLead does not have support team yet.");
            }

            var team = await dbContext.SupportTeams.FirstOrDefaultAsync(t => t.Id == lead.SupportTeamId.Value, cancellationToken);

            if (team is null)
            {
                throw new NotFoundException("Support team not found.");
            }
            var tickeAssignments = await dbContext.TicketAssignments.AsNoTracking().Where(a => a.TeamId == team.Id && a.IsActive)
            .Select(a => new
            {
                a.AgentId,
                AgentName = a.Agent.FullName,
                TicketId = a.TicketId,
                a.Ticket.Status
            }).ToListAsync(cancellationToken);

            var totalAssignedTickets = tickeAssignments.Select(a => a.TicketId).Distinct().Count();
            var totalInProgressTickets = tickeAssignments.Count(a => a.Status == TicketStatus.InProgress);
            var totalResolvedTickets = tickeAssignments.Count(a => a.Status == TicketStatus.Resolved);
            var totalClosedTickets = tickeAssignments.Count(a => a.Status == TicketStatus.Closed);
            var totalCancelledTickets = tickeAssignments.Count(a => a.Status == TicketStatus.Cancelled);
            var totalActiveTickets = tickeAssignments.Count(a => a.Status != TicketStatus.Resolved && a.Status != TicketStatus.Closed && a.Status != TicketStatus.Cancelled);

            var agents = await dbContext.AgentProfiles.AsNoTracking().Where(a => a.SupportTeamId == team.Id && a.IsActive && a.UserId != UserId).Select(a => new TeamMemberQueueDto
            {
                AgentId = a.Id,
                AgentName = a.FullName,
                TotalAssignedTickets = a.Assignments.Count(a => a.IsActive && a.TeamId == team.Id),
                ActiveTickets = a.Assignments.Count(a => a.IsActive && a.TeamId == team.Id && a.Ticket.Status != TicketStatus.Resolved && a.Ticket.Status != TicketStatus.Closed && a.Ticket.Status != TicketStatus.Cancelled),
                ResolvedTickets = a.Assignments.Count(a => a.IsActive && a.TeamId == team.Id && a.Ticket.Status == TicketStatus.Resolved),
                InProgressTickets = a.Assignments.Count(a => a.IsActive && a.TeamId == team.Id && a.Ticket.Status == TicketStatus.InProgress),
                ClosedOrCacelledTickets = a.Assignments.Count(a => a.IsActive && a.TeamId == team.Id && (a.Ticket.Status == TicketStatus.Cancelled || a.Ticket.Status == TicketStatus.Closed))
            })
            .ToListAsync(cancellationToken);

            return new GetMyTeamQueueResult
            {
                Team = new GetMyTeamQueueResponseDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    IsActive = team.IsActive,
                    TotalAgents = agents.Count,
                    TotalAssignedTickets = totalAssignedTickets,
                    TotalInProgressTickets = totalInProgressTickets,
                    TotalResolvedTickets = totalResolvedTickets,
                    TotalClosedTickets = totalClosedTickets,
                    TotalCancelledTickets = totalCancelledTickets,
                    TotalActiveTickets = totalActiveTickets,
                    Agents = agents
                }
            };

        }
    }
}