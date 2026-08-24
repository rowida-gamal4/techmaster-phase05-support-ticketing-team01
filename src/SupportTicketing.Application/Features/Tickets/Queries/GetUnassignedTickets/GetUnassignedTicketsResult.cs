using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetUnassignedTickets;

public class GetUnassignedTicketsResult
{
    public List<UnassignedTicketDto> Tickets { get; set; } = new();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}