using FluentValidation;

namespace SupportTicketing.Application.Features.Comments.Commands.AddCustomerComment
{
    public class AddCustomerCommentValidator : AbstractValidator<AddCustomerCommentCommand>
    {
        public AddCustomerCommentValidator()
        {
            RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0.");

            RuleFor(x => x.Request.Content).NotEmpty().WithMessage("Comment content is required.").MaximumLength(4000).WithMessage("Comment cannot exceed 4000 characters.");
        }
    }
}