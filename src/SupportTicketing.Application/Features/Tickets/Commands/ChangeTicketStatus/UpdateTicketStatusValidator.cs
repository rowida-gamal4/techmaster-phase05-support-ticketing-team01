using FluentValidation;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Commands.ChangeTicketStatus
{
    public class UpdateTicketStatusValidator : AbstractValidator<UpdateTicketStatusCommand>
    {
        public UpdateTicketStatusValidator()
        {
            RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("TicketId must be greater than 0.");

            RuleFor(x => x.Request.Status).NotEmpty().WithMessage("Status is required.").Must(ValidStatus).WithMessage("Invalid ticket status.");
        }

        private static bool ValidStatus(string status)
        {
            return Enum.TryParse<TicketStatus>(status,true, out _);
        }
    }
}