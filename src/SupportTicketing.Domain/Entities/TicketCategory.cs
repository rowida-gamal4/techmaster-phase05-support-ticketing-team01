using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class TicketCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<SlaPolicy> SlaPolicies { get; set; } = new List<SlaPolicy>();

}