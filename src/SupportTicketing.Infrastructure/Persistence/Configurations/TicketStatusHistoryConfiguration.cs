using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class TicketStatusHistoryConfiguration : IEntityTypeConfiguration<TicketStatusHistory>
{
	public void Configure(EntityTypeBuilder<TicketStatusHistory> builder)
	{
		builder.ToTable("TicketStatusHistories");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Reason)
			.HasMaxLength(1000);
		builder.HasOne(x => x.Ticket)
			.WithMany(x => x.StatusHistory)
			.HasForeignKey(x => x.TicketId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasOne(x => x.ChangedByUser)
			.WithMany()
			.HasForeignKey(x => x.ChangedByUserId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}