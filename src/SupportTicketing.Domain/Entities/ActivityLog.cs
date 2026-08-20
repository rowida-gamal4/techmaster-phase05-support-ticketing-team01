using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public int UserId {  get; set; }
    public string EntityName {get; set;}
    public int EntityId { get; set; }
    public string Action { get; set;}
    public ApplicationUser User { get; set; }
}