using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class SupportTeamConfiguration : IEntityTypeConfiguration<SupportTeam>
{
    public void Configure(EntityTypeBuilder<SupportTeam> builder)
    {
        builder.ToTable("SupportTeams");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.IsActive).IsRequired();
    }
}