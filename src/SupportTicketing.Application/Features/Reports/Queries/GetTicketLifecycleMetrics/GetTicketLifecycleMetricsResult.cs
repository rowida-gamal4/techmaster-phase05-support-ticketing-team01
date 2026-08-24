namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketLifecycleMetrics;

public class GetTicketLifecycleMetricsResult
{
    public int NewCount { get; set; }
    public int InProgressCount { get; set; }
    public int ResolvedCount { get; set; }
    public int ClosedCount { get; set; }
    public int CancelledCount { get; set; }
    public int TotalCount { get; set; }
}