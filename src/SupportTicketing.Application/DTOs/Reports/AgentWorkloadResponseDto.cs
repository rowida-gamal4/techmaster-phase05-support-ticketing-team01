namespace SupportTicketing.Application.DTOs.Reports
{
    public class AgentWorkloadResponseDto
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = null!;
        public int TotalAssignedTickets { get; set; }
        public int ActiveTickets { get; set; }
    }
}

