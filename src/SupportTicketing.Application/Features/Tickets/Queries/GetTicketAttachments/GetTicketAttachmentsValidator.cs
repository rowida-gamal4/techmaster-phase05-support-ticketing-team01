using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetTicketAttachments
{
    public class GetTicketAttachmentsValidator : AbstractValidator<GetTicketAttachmentsQuery>
    {
        public GetTicketAttachmentsValidator()
        {
            RuleFor(x => x.Request.TicketId).GreaterThan(0).When(x => x.Request.TicketId.HasValue).WithMessage("Ticket ID must be greater than 0.");
        }
    }
}

