using MediatR;

namespace SupportTicketing.Application.Features.Agent.Queries.GetMyActiveTickets;

public record GetMyActiveTicketsQuery(
    string SortBy = "priority"
) : IRequest<GetMyActiveTicketsResult>;