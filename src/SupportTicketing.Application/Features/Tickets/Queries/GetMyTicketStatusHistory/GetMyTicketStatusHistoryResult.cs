using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketStatusHistory;

public class GetMyTicketStatusHistoryResult
{
    public int TicketId { get; set; }

    public List<MyTicketStatusHistoryDto> History { get; set; } = new();
}