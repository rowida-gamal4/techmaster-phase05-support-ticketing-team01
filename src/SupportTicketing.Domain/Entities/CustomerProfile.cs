using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;


namespace SupportTicketing.Domain.Entities;

public class CustomerProfile : BaseEntity
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public ApplicationUser User { get; private set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
