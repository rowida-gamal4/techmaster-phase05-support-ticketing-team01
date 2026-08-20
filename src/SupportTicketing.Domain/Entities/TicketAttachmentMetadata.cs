using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class TicketAttachmentMetadata : BaseEntity
{
    public int TicketId {  get; set; }
    public int UploadedByUserId { get; set; }
    public string FileName {  get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public string StorageKey { get; set; }
    public Ticket Ticket { get; set; }
    public ApplicationUser UploadedByUser { get; private set; } = null!;
}