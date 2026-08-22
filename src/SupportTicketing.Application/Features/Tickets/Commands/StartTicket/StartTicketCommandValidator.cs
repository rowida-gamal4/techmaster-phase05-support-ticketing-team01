using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Commands.StartTicket;

public class StartTicketCommandValidator : AbstractValidator<StartTicketCommand>
{
	public StartTicketCommandValidator()
	{
		RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0.");
	}
}