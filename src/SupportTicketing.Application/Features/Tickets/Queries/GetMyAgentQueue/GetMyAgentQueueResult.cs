using SupportTicketing.Application.DTOs.TicketAssignment;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyAgentQueue;

public class GetMyAgentQueueResult
{
    public List<AgentQueueItemDto> Tickets { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}