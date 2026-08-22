using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Commands.ChangeTicketStatus;

public class UpdateTicketStatusResult
{
    public TicketStatusResponseDto Ticket { get; set; } = null!;
}