namespace SupportTicketing.Application.DTOs.Tickets;

public class CancelTicketRequestDto
{
    public string CancellationReason { get; set; } = string.Empty;
}