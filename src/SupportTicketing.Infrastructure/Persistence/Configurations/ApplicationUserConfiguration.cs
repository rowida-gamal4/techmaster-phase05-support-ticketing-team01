using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.FullName).IsRequired().HasMaxLength(150);
        builder.Property(t => t.Email).IsRequired().HasMaxLength(256);
        builder.Property(t => t.PasswordHash).IsRequired().HasMaxLength(100);

    }
}