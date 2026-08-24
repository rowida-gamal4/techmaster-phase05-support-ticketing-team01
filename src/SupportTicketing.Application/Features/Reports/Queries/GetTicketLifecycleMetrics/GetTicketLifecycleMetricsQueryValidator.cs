using FluentValidation;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketLifecycleMetrics;

public class GetTicketLifecycleMetricsQueryValidator
    : AbstractValidator<GetTicketLifecycleMetricsQuery>
{
    public GetTicketLifecycleMetricsQueryValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue).WithMessage("CategoryId must be greater than 0");

        RuleFor(x => x).Must(x => !x.FromDate.HasValue ||
                       !x.ToDate.HasValue ||
                       x.FromDate.Value <= x.ToDate.Value).WithMessage("FromDate must be earlier than or equal to ToDate.");
    }
}