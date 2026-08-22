using MediatR;
using SupportTicketing.Application.DTOs.TicketAssignment;

namespace SupportTicketing.Application.Features.Tickets.Commands.AssignTicket;

public record AssignTicketCommand(int TicketId, AssignTicketRequestDto Request) : IRequest<AssignTicketResult>;