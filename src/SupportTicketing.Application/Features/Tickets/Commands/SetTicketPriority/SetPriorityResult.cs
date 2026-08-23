namespace SupportTicketing.Application.Features.Tickets.Commands.SetTicketPriority;

public class SetPriorityResult
{
	public int TicketId {  get; set; }
	public string Priority { get; set; } = string.Empty;
    public string Status {  get; set; } = string.Empty;
}