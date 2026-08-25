using MediatR;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetUnassignedTickets;

public record GetUnassignedTicketsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null,
    string? Priority = null,
    string SortBy = "priority"
) : IRequest<GetUnassignedTicketsResult>;