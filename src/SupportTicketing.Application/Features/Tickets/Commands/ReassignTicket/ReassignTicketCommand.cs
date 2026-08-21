using MediatR;
using SupportTicketing.Application.DTOs.TicketAssignment;

namespace SupportTicketing.Application.Features.Tickets.Commands.ReassignTicket;

public record ReassignTicketCommand(int TicketId,ReassignTicketRequestDto Request) : IRequest<ReassignTicketResult>;