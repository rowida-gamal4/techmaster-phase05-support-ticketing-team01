using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicket;

public class GetMyTicketQueryValidator : AbstractValidator<GetMyTicketQuery>
{
    public GetMyTicketQueryValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0");
    }
}