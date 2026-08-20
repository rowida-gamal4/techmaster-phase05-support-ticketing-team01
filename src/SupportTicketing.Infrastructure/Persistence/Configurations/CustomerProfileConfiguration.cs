using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.ToTable("CustomerProfiles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PhoneNumber).HasMaxLength(30);
        builder.HasOne(x => x.User).WithOne(x=>x.CustomerProfile).HasForeignKey<CustomerProfile>(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}