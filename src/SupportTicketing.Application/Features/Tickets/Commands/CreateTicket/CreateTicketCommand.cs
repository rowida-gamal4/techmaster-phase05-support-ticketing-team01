using MediatR;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Commands.CreateTicket
{
    public record CreateTicketCommand(CreateTicketRequestDto Request) : IRequest<CreateTicketResult>;
}