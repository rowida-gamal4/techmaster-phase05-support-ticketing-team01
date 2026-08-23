using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketStatusHistory;

public class GetMyTicketStatusHistoryQueryValidator
    : AbstractValidator<GetMyTicketStatusHistoryQuery>
{
    public GetMyTicketStatusHistoryQueryValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0.");
    }
}