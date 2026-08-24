using MediatR;

namespace SupportTicketing.Application.Features.Reports.Queries.GetAgentWorkloadReport
{
    public record GetAgentWorkloadQuery : IRequest<GetAgentWorkloadResult>;
   
}