using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Commands.ReassignTicket;

public class ReassignTicketCommandValidator : AbstractValidator<ReassignTicketCommand>
{
    public ReassignTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0");

        RuleFor(x => x.Request.AgentId).GreaterThan(0).WithMessage("AgentId must be greater than 0");
        
        RuleFor(x => x.Request.TeamId).GreaterThan(0).WithMessage("TicketId must be greater than 0");

    }
}