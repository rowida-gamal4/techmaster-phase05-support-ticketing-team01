using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class SlaPolicyConfiguration : IEntityTypeConfiguration<SlaPolicy>
{
    public void Configure(EntityTypeBuilder<SlaPolicy> builder)
    {
        builder.ToTable("SlaPolicies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Priority)
            .IsRequired();
        builder.Property(x => x.ResponseTimeMin)
            .IsRequired();
        builder.Property(x => x.ResolutionTimeMin)
            .IsRequired();
        builder.HasOne(x => x.Category)
            .WithMany(x => x.SlaPolicies)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}