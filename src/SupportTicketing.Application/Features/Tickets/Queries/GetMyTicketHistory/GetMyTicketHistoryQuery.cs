using MediatR;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketHistory;

public record GetMyTicketHistoryQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null,
    string? Search = null
) : IRequest<GetMyTicketHistoryResult>;