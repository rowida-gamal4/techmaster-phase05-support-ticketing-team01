using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Exceptions;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketsByStatusReport
{



    public class GetTicketsByStatusQueryHandler : IRequestHandler<GetTicketsByStatusQuery, GetTicketsByStatusResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;

        public GetTicketsByStatusQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
        }

        public async Task<GetTicketsByStatusResult> Handle(GetTicketsByStatusQuery request, CancellationToken cancellationToken)
        {
            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required.");
            }

            if (request.Request.FromDate.HasValue && request.Request.ToDate.HasValue && request.Request.FromDate > request.Request.ToDate)
            {
                throw new ArgumentException("FromDate cannot be greater than ToDate.");
            }

            var query = dbContext.Tickets.AsNoTracking();


            if (request.Request.FromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= request.Request.FromDate.Value);
            }

            if (request.Request.ToDate.HasValue)
            {
                var toDate = request.Request.ToDate.Value.Date.AddDays(1);

                query = query.Where(t => t.CreatedAt < toDate);
            }


            var byStatus = await query.GroupBy(t => t.Status).Select(g => new TicketStatusCountDto
            {
                Status = g.Key.ToString(),
                Count = g.Count()
            }).OrderByDescending(t => t.Count).ToListAsync(cancellationToken);


            var byPriority = await query.GroupBy(t => t.Priority).Select(g => new TicketPriorityCountDto
            {
                Priority = g.Key.ToString(),
                Count = g.Count()
            }).OrderByDescending(t => t.Count).ToListAsync(cancellationToken);

            return new GetTicketsByStatusResult
            {
                ByStatus = byStatus,
                ByPriority = byPriority
            };
        }
    }
}