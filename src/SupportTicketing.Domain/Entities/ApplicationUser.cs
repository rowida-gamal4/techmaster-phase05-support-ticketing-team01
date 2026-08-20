using SupportTicketing.Domain.Common;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Domain.Entities;

public class ApplicationUser : BaseEntity
{
	public string FullName { get; private set; }

	public string Email { get; private set; }

	public string PasswordHash { get; private set; }

	public CustomerProfile? CustomerProfile { get; private set; }

	public AgentProfile? AgentProfile { get; private set; }
}