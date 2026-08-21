using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketResult
{
    public TicketResponseDto Ticket { get; set; } = null!;
}