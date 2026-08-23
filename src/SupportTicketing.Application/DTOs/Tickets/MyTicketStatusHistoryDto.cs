namespace SupportTicketing.Application.DTOs.Tickets;

public class MyTicketStatusHistoryDto
{
    public int Id { get; set; }

    public string OldStatus { get; set; } = string.Empty;

    public string NewStatus { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; }

    public string? Reason { get; set; }
}