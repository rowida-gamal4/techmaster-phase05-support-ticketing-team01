using SupportTicketing.Application.DTOs.Agent;

namespace SupportTicketing.Application.Features.Agent.Queries.GetMyTeamQueue
{
    public class GetMyTeamQueueResult
    {
        public GetMyTeamQueueResponseDto Team { get; set; } = null!;
    }
}