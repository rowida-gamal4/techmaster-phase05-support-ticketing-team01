namespace SupportTicketing.Application.DTOs.Reports
{
    public class AuditLogItemDto
    {
        public int TicketId { get; set; }

        public string Action { get; set; } = string.Empty;

        public int PerformedByUserId { get; set; }

        public string PerformedBy { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? Reason { get; set; }

        public DateTime PerformedAt { get; set; }
    }
}