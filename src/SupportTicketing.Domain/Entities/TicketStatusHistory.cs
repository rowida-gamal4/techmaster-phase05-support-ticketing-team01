using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class TicketStatusHistory : BaseEntity
{
    public int TicketId { get; set; }
    public int ChangedByUserId { get; set; }
    public DateTime ChangedAt { get; set; }
    public TicketStatus OldStatus { get; set; }
    public TicketStatus NewStatus { get; set; }
    public string? Reason { get; set; }
    public Ticket Ticket { get; set; }
    public ApplicationUser ChangedByUser { get; set; }
}