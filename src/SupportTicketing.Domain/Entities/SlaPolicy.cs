using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class SlaPolicy : BaseEntity
{
    public int CategoryId { get; set; }
    public TicketPriority Priority { get; set; }
    public int ResponseTimeMin { get; set; }
    public int ResolutionTimeMax { get; set; }
    public bool IsActive { get; set; }
    public TicketCategory Category { get; set; }

}