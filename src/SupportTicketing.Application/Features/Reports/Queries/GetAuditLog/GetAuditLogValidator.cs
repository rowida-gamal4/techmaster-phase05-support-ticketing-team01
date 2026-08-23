using FluentValidation;

namespace SupportTicketing.Application.Features.Reports.GetAuditLog
{
    public class GetAuditLogValidator : AbstractValidator<GetAuditLogQuery>
    {
        public GetAuditLogValidator()
        {
            RuleFor(x => x.Request.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Request.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.Request).Must(x => !x.FromDate.HasValue ||!x.ToDate.HasValue ||x.FromDate.Value <= x.ToDate.Value).WithMessage("FromDate must be earlier than or equal to ToDate.");
        }
    }
}