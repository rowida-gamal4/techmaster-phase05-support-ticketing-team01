using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicket;

public class GetMyTicketResult
{
	public MyTicketDetailsDto Ticket { get; set; } = null!;
}