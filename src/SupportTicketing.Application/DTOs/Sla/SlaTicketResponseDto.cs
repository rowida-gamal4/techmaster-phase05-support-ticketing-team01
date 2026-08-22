namespace SupportTicketing.Application.DTOs.Sla
{
    public class SlaTicketResponseDto
    {
        public int TicketId { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime SlaTargetAt { get; set; }

        public bool IsBreached { get; set; }

        public int MinutesRemaining { get; set; }
    }
}