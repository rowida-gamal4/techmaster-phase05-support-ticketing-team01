using MediatR;

namespace SupportTicketing.Application.Features.Tickets.Commands.StartTicket;

public record StartTicketCommand(int TicketId) : IRequest<StartTicketResult>;