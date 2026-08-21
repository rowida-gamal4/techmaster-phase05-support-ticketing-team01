using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Commands.CancelTicket
{
    public class CancelTicketCommandValidator : AbstractValidator<CancelTicketCommand>
    {
        public CancelTicketCommandValidator()
        {
            RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("Ticket ID must be greater than 0.");

            RuleFor(x => x.Request.CancellationReason).NotEmpty().WithMessage("Cancellation reason is required.").MaximumLength(1000).WithMessage("Cancellation reason can not exceed 1000 characters.");
        }
    }
}