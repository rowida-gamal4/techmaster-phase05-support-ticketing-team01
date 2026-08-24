using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.Queries.GetHighPriorityOpenTickets
{
    public class GetHighPriorityOpenTicketsResult
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<HighPriorityTicketDto> Tickets { get; set; } = new();

    }
}