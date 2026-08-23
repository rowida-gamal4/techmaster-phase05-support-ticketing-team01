using MediatR;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketStatusHistory;

public record GetMyTicketStatusHistoryQuery(
    int TicketId
) : IRequest<GetMyTicketStatusHistoryResult>;