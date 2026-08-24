namespace SupportTicketing.Application.DTOs.Tickets
{
    public class TicketAttachmentMetadataResponseDto
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public int UploadedByUserId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public string StorageKey { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}