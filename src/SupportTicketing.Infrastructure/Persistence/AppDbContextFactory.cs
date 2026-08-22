using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SupportTicketing.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost;Database=SupportTicketing ;User Id=sa;Password=Admin@123!;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}