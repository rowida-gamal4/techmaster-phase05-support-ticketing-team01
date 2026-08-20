using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
	public void Configure(EntityTypeBuilder<Ticket> builder)
	{
		builder.ToTable("Tickets");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
		builder.Property(x => x.Description).IsRequired().HasMaxLength(5000);
		builder.Property(x => x.Priority).IsRequired();
		builder.Property(x => x.Status).IsRequired();
		builder.HasOne(x => x.Customer).WithMany(x => x.Tickets).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
		builder.HasOne(x => x.Category).WithMany(x => x.Tickets).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
	}
}