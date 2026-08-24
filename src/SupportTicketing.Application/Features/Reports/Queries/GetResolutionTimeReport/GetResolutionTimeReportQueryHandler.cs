using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Reports.GetResolutionTimeReport;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Reports.GetResolutionTimeRepor
{
    public class GetResolutionTimeReportQueryHandler : IRequestHandler<GetResolutionTimeReportQuery, GetResolutionTimeReportResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;

        public GetResolutionTimeReportQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
        }
        public async Task<GetResolutionTimeReportResult> Handle(GetResolutionTimeReportQuery request, CancellationToken cancellationToken)
        {
            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required.");
            }
            var userRole = currentUserService.Role;

            if (userRole != Roles.Admin && currentUserService.Role != Roles.SupportLead)
            {
                throw new ForbiddenException("You are not allowed to see this report.");
            }

            var tickets = await dbContext.Tickets.Where(t => (t.Status == TicketStatus.Resolved || t.Status == TicketStatus.Closed) && t.ResolvedAt.HasValue && t.ResolvedAt.Value >= t.CreatedAt).Select(t => new
            {
                t.Id,
                t.Title,
                t.Status,
                t.CreatedAt,
                ResolvedAt = t.ResolvedAt!.Value
            }).ToListAsync(cancellationToken);

            var resolutionTickets = tickets.Select(t => new ResolutionTicketDto
            {
                TicketId = t.Id,
                Title = t.Title,
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                ResolvedAt = t.ResolvedAt,
                ResolutionTimeMinutes = (t.ResolvedAt - t.CreatedAt).TotalMinutes
            }).OrderByDescending(t => t.ResolutionTimeMinutes).ToList();

            var resolutionTimes = resolutionTickets.Select(t => t.ResolutionTimeMinutes).ToList();

            var hasValue = resolutionTimes.Count > 0;

            return new GetResolutionTimeReportResult
            {
                TotalResolvedTickets = resolutionTickets.Count,

                AverageResolutionTimeMinutes = hasValue ? Math.Round(resolutionTimes.Average(), 2) : 0,

                AverageResolutionTimeHours = hasValue ? Math.Round(resolutionTimes.Average() / 60, 2) : 0,

                FastestResolutionTimeMinutes = hasValue ? Math.Round(resolutionTimes.Min(), 2) : 0,

                FastestResolutionTimeHours = hasValue ? Math.Round(resolutionTimes.Min() / 60, 2) : 0,

                LongestResolutionTimeMinutes = hasValue ? Math.Round(resolutionTimes.Max(), 2) : 0,

                LongestResolutionTimeHours = hasValue ? Math.Round(resolutionTimes.Max() / 60, 2) : 0,
                Tickets = resolutionTickets
            };

        }
    }
}