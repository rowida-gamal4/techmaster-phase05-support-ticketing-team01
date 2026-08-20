using Microsoft.AspNetCore.Identity;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Infrastructure.Seed;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        string[] roles =
        {
            Roles.Admin,
            Roles.SupportLead,
            Roles.SupportAgent,
            Roles.Customer
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<int>(role));
            }
        }
    }
}