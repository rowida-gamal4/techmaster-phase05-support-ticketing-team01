using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class AgentProfileConfiguration : IEntityTypeConfiguration<AgentProfile>
{
	public void Configure(EntityTypeBuilder<AgentProfile> builder)
	{
		builder.ToTable("AgentProfiles");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.IsActive).IsRequired();
		builder.HasOne(x => x.User).WithOne(x => x.AgentProfile).HasForeignKey<AgentProfile>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
		builder.HasOne(x => x.SupportTeam).WithMany(x => x.Agents).HasForeignKey(x => x.SupportTeamId).OnDelete(DeleteBehavior.SetNull);
	}
}