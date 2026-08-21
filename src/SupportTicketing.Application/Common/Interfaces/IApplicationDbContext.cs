using Microsoft.EntityFrameworkCore;
using SupportTicketing.Domain.Entities;
namespace SupportTicketing.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> ApplicationUsers { get; }
        DbSet<CustomerProfile> CustomerProfiles { get; }
        DbSet<AgentProfile> AgentProfiles { get; }
        DbSet<SupportTeam> SupportTeams { get; }
        DbSet<Ticket> Tickets { get; }
        DbSet<TicketCategory> TicketCategories { get; }
        DbSet<TicketComment> TicketComments { get; }
        DbSet<TicketAttachmentMetadata> TicketAttachments { get; }
        DbSet<TicketAssignment> TicketAssignments { get; }
        DbSet<TicketStatusHistory> TicketStatusHistories { get; }
        DbSet<SlaPolicy> SlaPolicies { get; }
        DbSet<ActivityLog> ActivityLogs { get; }

        Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
    }


}