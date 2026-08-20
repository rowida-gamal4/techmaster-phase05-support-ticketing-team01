using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class Ticket : BaseEntity
{
    public int CategoryId {  get; set; }
    public int CustomerId {  get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public CustomerProfile Customer { get; set; } = null!;
    public TicketCategory Category { get; set; } = null!;
    public ICollection<TicketAssignment> Assignments { get; set; } = new List<TicketAssignment>();
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public ICollection<TicketStatusHistory> StatusHistory { get; set; } = new List<TicketStatusHistory>();
    public ICollection<TicketAttachmentMetadata> Attachments { get; set; } = new List<TicketAttachmentMetadata>();


}