namespace SupportTicketing.Application.DTOs.Tickets
{
    public class TicketStatusResponseDto
    {
        public int TicketId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}