using MediatR;
using SupportTicketing.Application.DTOs.TicketTriage;

namespace SupportTicketing.Application.Features.Tickets.Commands.SetTicketPriority;

public record SetPriorityCommand(int TicketId, SetPriorityRequestDto Request) : IRequest<SetPriorityResult>;