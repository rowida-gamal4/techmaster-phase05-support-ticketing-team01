using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Commands.AddTicketAttachmentMetadata
{
    public class AddTicketAttachmentMetadataResult
    {
        public TicketAttachmentMetadataResponseDto Attachment { get; set; } = null!;
    }
}

