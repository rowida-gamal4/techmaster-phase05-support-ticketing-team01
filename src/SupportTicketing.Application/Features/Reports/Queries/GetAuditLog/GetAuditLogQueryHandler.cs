using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Reports.GetAuditLog
{
    public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, GetAuditLogResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly IValidator<GetAuditLogQuery> validator;

        public GetAuditLogQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<GetAuditLogQuery> validator)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.validator = validator;
        }
        public async Task<GetAuditLogResult> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required");
            }

            if (currentUserService.Role != Roles.Admin)
            {
                throw new ForbiddenException("Only Admin can view audit logs.");
            }

            var activityQuery = dbContext.ActivityLogs.AsNoTracking().Where(a => a.EntityName == "Ticket");
            var statusHistoryQuery = dbContext.TicketStatusHistories.AsNoTracking();

            if (request.Request.TicketId.HasValue)
            {
                var ticketId = request.Request.TicketId.Value;
                activityQuery = activityQuery.Where(x => x.EntityName == "Ticket" && x.EntityId == ticketId);
                statusHistoryQuery = statusHistoryQuery.Where(x => x.TicketId == ticketId);
            }

            if (request.Request.FromDate.HasValue)
            {
                var fromDate = request.Request.FromDate.Value;
                activityQuery = activityQuery.Where(x => x.CreatedAt >= fromDate);
                statusHistoryQuery = statusHistoryQuery.Where(x => x.ChangedAt >= fromDate);
            }

            if (request.Request.ToDate.HasValue)
            {
                var toDate = request.Request.ToDate.Value.Date.AddDays(1);
                activityQuery = activityQuery.Where(x => x.CreatedAt < toDate);
                statusHistoryQuery = statusHistoryQuery.Where(x => x.ChangedAt < toDate);
            }

            if (!string.IsNullOrWhiteSpace(request.Request.Action))
            {
                activityQuery = activityQuery.Where(a => a.Action == request.Request.Action);
            }

            var activityCount = await activityQuery.CountAsync(cancellationToken);
            var statusCount = await statusHistoryQuery.CountAsync(cancellationToken);
            var totalCount = activityCount + statusCount;

            var pageNumber = request.Request.PageNumber;
            var pageSize = request.Request.PageSize;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var assignmentActions = await activityQuery.OrderByDescending(a => a.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(a => new AuditLogItemDto
            {
                TicketId = a.EntityId,
                Action = a.Action,
                PerformedByUserId = a.UserId,
                PerformedBy = a.User.FullName,
                PerformedAt = a.CreatedAt
            }) .ToListAsync(cancellationToken);


            var statusHistory = await statusHistoryQuery.OrderByDescending(s => s.ChangedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(s => new StatusHistoryItemDto
            {
                TicketId = s.TicketId,
                OldStatus = s.OldStatus.ToString(),
                NewStatus = s.NewStatus.ToString(),
                Reason = s.Reason,
                ChangedByUserId = s.ChangedByUserId,
                ChangedBy = s.ChangedByUser.FullName,
                ChangedAt = s.ChangedAt
            }).ToListAsync(cancellationToken);

            return new GetAuditLogResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                AssignmentActions = assignmentActions,
                StatusChanges = statusHistory
            };
        }
    }
}