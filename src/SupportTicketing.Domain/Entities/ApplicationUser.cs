using Microsoft.AspNetCore.Identity;
using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{

    public string FullName { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public CustomerProfile? CustomerProfile { get; set; }

    public AgentProfile? AgentProfile { get; set; }

}