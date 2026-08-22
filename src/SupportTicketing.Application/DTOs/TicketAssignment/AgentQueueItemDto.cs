namespace SupportTicketing.Application.DTOs.TicketAssignment;

public class AgentQueueItemDto
{
    public int TicketId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime AssignedAt { get; set; }
}