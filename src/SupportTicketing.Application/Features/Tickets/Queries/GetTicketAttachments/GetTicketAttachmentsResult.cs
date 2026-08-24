using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetTicketAttachments
{
    public class GetTicketAttachmentsResult
    {
        public List<TicketAttachmentMetadataResponseDto> Attachments { get; set; } = new();
    }
}