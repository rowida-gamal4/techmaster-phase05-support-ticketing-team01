using FluentValidation;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.SetTicketPriority;

public class SetPriorityCommandValidator : AbstractValidator<SetPriorityCommand>
{
    public SetPriorityCommandValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0");
        
        RuleFor(x => x.Request.Priority).Must(BeValidPriority).WithMessage("Invalid ticket priority");

    }
    private static bool BeValidPriority(int priority)
    {
        return Enum.IsDefined(typeof(TicketPriority), priority);
    }
}