using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class TicketComment : BaseEntity
{
    public int TicketId { get; set; }
    public int AuthorUserId { get; set; }
    public string Content { get; set; }
    public CommentVisibility Visibility { get; set; }
    public Ticket Ticket {  get; set; }
    public ApplicationUser Author {  get; set; }
}