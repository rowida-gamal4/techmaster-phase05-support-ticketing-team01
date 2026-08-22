using FluentValidation;

namespace SupportTicketing.Application.Features.Comments.Commands.AddAgentPublicReply
{



    public class AddAgentPublicReplyValidator : AbstractValidator<AddAgentPublicReplyCommand>
    {
        public AddAgentPublicReplyValidator()
        {
            RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0.");

            RuleFor(x => x.Request.Content).NotEmpty().WithMessage("Reply content is required.").MaximumLength(4000).WithMessage("Reply can not exceed 4000 characters.");
        }
    }
}