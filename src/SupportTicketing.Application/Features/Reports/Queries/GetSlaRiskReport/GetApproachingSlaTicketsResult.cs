using SupportTicketing.Application.DTOs.Sla;

namespace SupportTicketing.Application.Features.Sla.Queries.GetSlaRiskReport
{
    public class GetApproachingSlaTicketsResult
    {
        public List<SlaTicketResponseDto> Tickets { get; set; } = new();
    }
}