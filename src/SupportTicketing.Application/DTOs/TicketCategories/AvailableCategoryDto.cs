namespace SupportTicketing.Application.DTOs.TicketCategories;

public class AvailableCategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? Description { get; set; }
}