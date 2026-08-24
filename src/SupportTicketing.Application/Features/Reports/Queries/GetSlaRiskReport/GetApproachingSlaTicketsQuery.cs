using MediatR;

namespace SupportTicketing.Application.Features.Sla.Queries.GetSlaRiskReport
{
    public record GetApproachingSlaTicketsQuery : IRequest<GetApproachingSlaTicketsResult>;
}