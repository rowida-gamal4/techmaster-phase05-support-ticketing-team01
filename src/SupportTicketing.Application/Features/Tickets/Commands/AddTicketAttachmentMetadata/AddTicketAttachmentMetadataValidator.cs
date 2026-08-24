using FluentValidation;

namespace SupportTicketing.Application.Features.Tickets.Commands.AddTicketAttachmentMetadata
{
    public class AddTicketAttachmentMetadataValidator : AbstractValidator<AddTicketAttachmentMetadataCommand>
    {
        public AddTicketAttachmentMetadataValidator()
        {
            RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("Ticket ID must be greater than 0.");

            RuleFor(x => x.Request.FileName).NotEmpty().MaximumLength(255);

            RuleFor(x => x.Request.FileSize).GreaterThan(0).WithMessage("File size must be greater than 0.");

            RuleFor(x => x.Request.ContentType).NotEmpty().Must(type => new[]
            {
                 "image/jpeg",
                 "image/png",
                 "application/pdf"
            }.Contains(type.ToLowerInvariant())).WithMessage("File type must be JPG, PNG, or PDF.");

        }
    }
}