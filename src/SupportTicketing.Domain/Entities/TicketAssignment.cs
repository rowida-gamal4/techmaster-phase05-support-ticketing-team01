using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class TicketAssignment : BaseEntity
{
    public int AssignedByUserId {  get; set; }  
    public int TicketId { get; set; }
    public int AgentId {  get; set; }
    public int TeamId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsActive { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public AgentProfile Agent { get; set; } = null!;
    public SupportTeam? Team { get; set; } = null!;
    public ApplicationUser AssignedByUser { get; set; } = null!;
}