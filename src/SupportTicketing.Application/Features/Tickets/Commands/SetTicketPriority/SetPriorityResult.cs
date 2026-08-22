namespace SupportTicketing.Application.Features.Tickets.Commands.SetTicketPriority;

public class SetPriorityResult
{
	public int TicketId {  get; set; }
	public string Priority { get; set; }
	public string Status {  get; set; }
}