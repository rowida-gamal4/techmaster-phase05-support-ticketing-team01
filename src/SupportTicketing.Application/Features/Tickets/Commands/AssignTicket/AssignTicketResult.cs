namespace SupportTicketing.Application.Features.Tickets.Commands.AssignTicket;

public class AssignTicketResult
{
    public int TicketId { get; set; }

    public int AgentId { get; set; }

    public int TeamId { get; set; }

    public DateTime AssignedAt { get; set; }

    public string Status { get; set; } = string.Empty;
}