using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.Infrastructure.Persistence.Configurations;

public class TicketAssignmentConfiguration : IEntityTypeConfiguration<TicketAssignment>
{
    public void Configure(EntityTypeBuilder<TicketAssignment> builder)
    {
        builder.ToTable("TicketAssignments");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Agent)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Team)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedByUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.TicketId);
        builder.HasIndex(x => x.AgentId);
        builder.HasIndex(x => new
        {
            x.AgentId,
            x.EndedAt
        });

    }
}