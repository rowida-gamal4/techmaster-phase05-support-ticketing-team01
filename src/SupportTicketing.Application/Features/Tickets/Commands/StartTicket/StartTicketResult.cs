namespace SupportTicketing.Application.Features.Tickets.Commands.StartTicket;

public class StartTicketResult
{
    public int TicketId {  get; set; }
    public string Status {  get; set; }
    public DateTime StartedAt { get; set; }
}