using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetUnassignedTickets;

public class GetUnassignedTicketsQueryValidator : AbstractValidator<GetUnassignedTicketsQuery>
{
    public GetUnassignedTicketsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);

        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);

        RuleFor(x => x.SortBy).Must(value =>
                value.Equals("priority", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortBy must be priority or createdAt.");
    }
}