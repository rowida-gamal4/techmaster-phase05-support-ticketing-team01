using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.Queries.GetAgentWorkloadReport
{
    public class GetAgentWorkloadResult
    {
        public List<AgentWorkloadResponseDto> Agents { get; set; } = new();
    }
}