namespace SupportTicketing.Application.DTOs.Reports
{
    public class StatusHistoryItemDto
    {
        public int TicketId { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public int ChangedByUserId { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}