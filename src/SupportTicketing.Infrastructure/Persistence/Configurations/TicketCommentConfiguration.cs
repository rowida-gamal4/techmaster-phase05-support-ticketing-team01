using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.ToTable("TicketComments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(5000);
        builder.Property(x => x.Visibility)
            .IsRequired();
        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}