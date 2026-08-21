using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyCustomerTickets;

public class GetMyCustomerTicketsValidator: AbstractValidator<GetMyCustomerTicketsQuery>
{
    public GetMyCustomerTicketsValidator()
    {
        RuleFor(x => x.Request.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.Request.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}