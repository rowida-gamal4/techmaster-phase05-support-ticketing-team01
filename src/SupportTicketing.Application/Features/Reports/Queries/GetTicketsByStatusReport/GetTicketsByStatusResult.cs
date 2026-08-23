using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketsByStatusReport
{

    public class GetTicketsByStatusResult
    {
        public List<TicketStatusCountDto> ByStatus { get; set; } = new();
        public List<TicketPriorityCountDto> ByPriority { get; set; } = new();
    }
}