using FluentValidation;

namespace SupportTicketing.Application.Features.Comments.Queries.GetMyTicketConversation;

public class GetMyTicketConversationQueryValidator : AbstractValidator<GetMyTicketConversationQuery>
{
    public GetMyTicketConversationQueryValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0");
        
        RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100).WithMessage("PageSize must be between 1 and 100");

    }
}