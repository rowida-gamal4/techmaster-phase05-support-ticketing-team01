using MediatR;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTopCustomersByTicketCount;

public record GetTopCustomersByTicketCountQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<GetTopCustomersByTicketCountResult>;