using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.GetAuditLog
{
    public class GetAuditLogResult
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public List<AuditLogItemDto> AssignmentActions { get; set; } = new();
        public List<StatusHistoryItemDto> StatusChanges { get; set; } = new();
    }
}