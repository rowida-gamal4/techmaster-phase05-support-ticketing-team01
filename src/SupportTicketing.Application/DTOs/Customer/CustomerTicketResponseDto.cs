namespace SupportTicketing.Application.DTOs.Customer;

public class CustomerTicketResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime? CancelledAt { get; set; }
}