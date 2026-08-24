namespace SupportTicketing.Application.DTOs.Reports
{
    public class GetAuditLogRequestDto
    {
        public int? TicketId { get; set; }

        public string? Action { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}