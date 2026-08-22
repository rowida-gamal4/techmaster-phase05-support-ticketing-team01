using MediatR;
using SupportTicketing.Application.DTOs.TicketTriage;

namespace SupportTicketing.Application.Features.Tickets.Commands.ResolveTicket;

public record ResolveTicketCommand(
    int TicketId,
    ResolveTicketRequestDto Request
) : IRequest<ResolveTicketResult>;