using MediatR;

namespace SupportTicketing.Application.Features.Sla.Queries.GetApproachingSlaTickets
{
    public record GetApproachingSlaTicketsQuery : IRequest<GetApproachingSlaTicketsResult>;
}