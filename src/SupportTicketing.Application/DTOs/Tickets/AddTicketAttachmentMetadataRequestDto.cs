namespace SupportTicketing.Application.DTOs.Tickets
{
    public class AddTicketAttachmentMetadataRequestDto
    {
        public string FileName { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string ContentType { get; set; } = string.Empty;

    }
}