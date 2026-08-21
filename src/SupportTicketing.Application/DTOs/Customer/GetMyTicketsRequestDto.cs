namespace SupportTicketing.Application.DTOs.Tickets;

public class GetMyTicketsRequestDto
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public int? CategoryId { get; set; }
}