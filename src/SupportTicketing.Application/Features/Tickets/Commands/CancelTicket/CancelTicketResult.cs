using SupportTicketing.Application.DTOs.Customer;
using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Commands.CancelTicket;

public class CancelTicketResult
{
    public CancelTicketResponseDTo Ticket { get; set; } = null!;
}