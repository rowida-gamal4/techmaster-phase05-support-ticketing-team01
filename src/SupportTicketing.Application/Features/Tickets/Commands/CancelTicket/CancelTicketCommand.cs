using MediatR;
using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Commands.CancelTicket
{
    public record CancelTicketCommand(int TicketId,CancelTicketRequestDto Request) : IRequest<CancelTicketResult>;
}