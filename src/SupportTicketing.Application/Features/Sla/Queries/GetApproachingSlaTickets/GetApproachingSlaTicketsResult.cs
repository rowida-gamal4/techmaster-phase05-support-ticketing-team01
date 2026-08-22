using SupportTicketing.Application.DTOs.Sla;

namespace SupportTicketing.Application.Features.Sla.Queries.GetApproachingSlaTickets
{
    public class GetApproachingSlaTicketsResult
    {
        public List<SlaTicketResponseDto> Tickets { get; set; } = new();
    }
}