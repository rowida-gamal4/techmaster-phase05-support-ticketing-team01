using MediatR;
using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Commands.ChangeTicketStatus;

public record UpdateTicketStatusCommand( int TicketId, UpdateTicketStatusRequestDto Request) : IRequest<UpdateTicketStatusResult>;