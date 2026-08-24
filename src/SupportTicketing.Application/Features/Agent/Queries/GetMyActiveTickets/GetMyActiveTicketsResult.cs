using SupportTicketing.Application.DTOs.Agent;

namespace SupportTicketing.Application.Features.Agent.Queries.GetMyActiveTickets;

public class GetMyActiveTicketsResult
{
    public List<MyActiveTicketDto> Tickets { get; set; } = new();
}