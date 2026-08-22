namespace SupportTicketing.Application.Features.Tickets.Commands.ReassignTicket;

public class ReassignTicketResult
{
    public int TicketId { get; set; }
    public int OldAgentId { get; set; }
    public int NewAgentId { get; set; }
    public int NewTeamId { get; set; }
    public DateTime ReassignedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
