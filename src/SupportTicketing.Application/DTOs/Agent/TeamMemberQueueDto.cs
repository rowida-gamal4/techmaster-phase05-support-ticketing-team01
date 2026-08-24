namespace SupportTicketing.Application.DTOs.Agent
{



    public class TeamMemberQueueDto
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int TotalAssignedTickets { get; set; }
        public int ActiveTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedOrCacelledTickets { get; set; }
       
    }
}