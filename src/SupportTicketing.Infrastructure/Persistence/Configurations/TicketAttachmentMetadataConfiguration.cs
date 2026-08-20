using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class TicketAttachmentMetadataConfiguration : IEntityTypeConfiguration<TicketAttachmentMetadata>
{
	public void Configure(EntityTypeBuilder<TicketAttachmentMetadata> builder)
	{
		builder.ToTable("TicketAttachments");
		builder.HasKey(t => t.Id);
		builder.Property(x => x.FileName)
			.IsRequired()
			.HasMaxLength(255);
		builder.Property(x=>x.ContentType)
			.IsRequired()
			.HasMaxLength(100);
		builder.Property(x => x.StorageKey)
			.IsRequired()
			.HasMaxLength(500);
		builder.HasOne(x=>x.Ticket).WithMany(x=>x.Attachments).HasForeignKey(x=>x.TicketId).OnDelete(DeleteBehavior.Cascade);
		builder.HasOne(x => x.UploadedByUser).WithMany().HasForeignKey(x => x.UploadedByUser).OnDelete(DeleteBehavior.Restrict);

	}
}