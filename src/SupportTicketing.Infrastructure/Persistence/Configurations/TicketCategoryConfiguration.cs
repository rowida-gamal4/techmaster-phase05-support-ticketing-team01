using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class TicketCategoryConfiguration : IEntityTypeConfiguration<TicketCategory>
{
	public void Configure(EntityTypeBuilder<TicketCategory> builder)
	{
		builder.ToTable("TicketCategories");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
		builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
		builder.Property(x => x.Description).HasMaxLength(500);
		builder.Property(x => x.IsActive).IsRequired();
		builder.HasIndex(x => x.Code).IsUnique();
	}
}