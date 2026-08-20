using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class AgentProfile : BaseEntity
{
    public int UserId {  get; set; }
    public bool IsActive { get; set; }
    public int? SupportTeamId { get; set; }
    public SupportTeam? SupportTeam { get; set; }
    public ApplicationUser User { get; private set; } = null!;
    public ICollection<TicketAssignment> Assignments { get; set; } = new List<TicketAssignment>();
}