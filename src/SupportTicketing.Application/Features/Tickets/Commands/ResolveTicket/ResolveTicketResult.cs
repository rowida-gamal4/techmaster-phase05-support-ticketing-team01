namespace SupportTicketing.Application.Features.Tickets.Commands.ResolveTicket;

public class ResolveTicketResult
{
    public int TicketId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ResolutionNotes { get; set; } = string.Empty;
    public DateTime ResolvedAt { get; set; }
}