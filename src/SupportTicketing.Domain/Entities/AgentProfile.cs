using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class AgentProfile : BaseEntity
{
    public bool IsActive { get; set; }
    public int? SupportTeamId { get; set; }
    public SupportTeam? SupportTeam { get; set; }
    public ICollection<TicketAssignment> Assignments { get; set; } = new List<TicketAssignment>();
}