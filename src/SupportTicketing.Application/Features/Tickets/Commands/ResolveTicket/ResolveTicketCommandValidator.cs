using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Commands.ResolveTicket;

public class ResolveTicketCommandValidator : AbstractValidator<ResolveTicketCommand>
{
    public ResolveTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0.");

        RuleFor(x => x.Request.ResolutionNotes).NotEmpty().WithMessage("Resolution notes are required")
            .MaximumLength(4000).WithMessage("Resolution notes cannot exceed 4000 characters");

    }
}