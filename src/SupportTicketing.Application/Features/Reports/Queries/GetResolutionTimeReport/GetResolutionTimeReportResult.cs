using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.GetResolutionTimeReport
{
    public class GetResolutionTimeReportResult
    {
        public int TotalResolvedTickets { get; set; }

        public double AverageResolutionTimeMinutes { get; set; }
        public double AverageResolutionTimeHours { get; set; }

        public double FastestResolutionTimeMinutes { get; set; }
        public double FastestResolutionTimeHours { get; set; }

        public double LongestResolutionTimeMinutes { get; set; }
        public double LongestResolutionTimeHours { get; set; }

        public List<ResolutionTicketDto> Tickets { get; set; } = new();
    }
}