using SupportTicketing.Application.DTOs.Customer;
namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyCustomerTickets;

public class GetMyCustomerTicketsResult
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
    public List<CustomerTicketResponseDto> Tickets { get; set; } = new();
}