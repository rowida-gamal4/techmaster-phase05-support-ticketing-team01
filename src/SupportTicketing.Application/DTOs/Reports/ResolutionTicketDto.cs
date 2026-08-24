namespace SupportTicketing.Application.DTOs.Reports
{
    public class ResolutionTicketDto
    {
        public int TicketId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ResolvedAt { get; set; }

        public double ResolutionTimeMinutes { get; set; }
    }
}