using MediatR;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicket;

public record GetMyTicketQuery(int TicketId) : IRequest<GetMyTicketResult>;