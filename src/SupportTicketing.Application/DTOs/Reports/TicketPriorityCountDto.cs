namespace SupportTicketing.Application.DTOs.Reports
{
    public class TicketPriorityCountDto
    {
        public string Priority { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}