using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class SupportTeam : BaseEntity
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public ICollection<TicketAssignment> Assignments { get; set; } = new List<TicketAssignment>();
}