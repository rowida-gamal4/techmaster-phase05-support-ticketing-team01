using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTopCustomersByTicketCount;

public class GetTopCustomersByTicketCountResult
{
    public List<TopCustomerDto> Customers { get; set; } = new();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}