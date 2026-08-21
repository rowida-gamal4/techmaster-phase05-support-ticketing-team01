using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommandValidator: AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(x=>x.Request.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(200).WithMessage("Title can not exceed 50 chars.");

            RuleFor(x => x.Request.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(4000).WithMessage("Description can not exceed 4000 chars.");

            RuleFor(x => x.Request.CategoryId).GreaterThan(0).WithMessage("CategoryId must be greater than 0 ."); 
            
        }
    }
}