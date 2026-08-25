using MediatR;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketLifecycleMetrics;

public record GetTicketLifecycleMetricsQuery(
    int? CategoryId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<GetTicketLifecycleMetricsResult>;