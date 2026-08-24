namespace SupportTicketing.Application.DTOs.Agent
{
    public class GetMyTeamQueueResponseDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public int TotalAgents { get; set; }
        public int TotalActiveTickets { get; set; }
        public int TotalAssignedTickets { get; set; }
        public int TotalInProgressTickets { get; set; }
        public int TotalResolvedTickets { get; set; }
        public int TotalClosedTickets { get; set; }
        public int TotalCancelledTickets { get; set; }
      

        public List<TeamMemberQueueDto> Agents { get; set; } = new();
    }
}