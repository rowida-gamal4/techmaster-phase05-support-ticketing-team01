using SupportTicketing.Application.DTOs.TicketCategories;

namespace SupportTicketing.Application.Features.Customer.Queries.GetAvailableCategories;

public class GetAvailableCategoriesResult
{
    public List<AvailableCategoryDto> Categories { get; set; } = new();
}