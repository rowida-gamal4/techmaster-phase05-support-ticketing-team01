using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Infrastructure.Persistence;

namespace SupportTicketing.IntegrationTests.Infrastructure;

public static class TestDataSeeder
{
    public static int CustomerUserId { get; private set; }
    public static int CategoryId { get; private set; }

    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        await context.Database.EnsureCreatedAsync();

        // ============================================
        // Create / Get Customer User
        // ============================================

        var customerUser = await userManager.FindByEmailAsync(
            "integration.customer@test.com");

        if (customerUser == null)
        {
            customerUser = new ApplicationUser
            {
                UserName = "integration.customer@test.com",
                Email = "integration.customer@test.com",
                FullName = "Integration Test Customer",
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(
                customerUser,
                "TestPassword123!");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e =>
                        $"{e.Code}: {e.Description}"));

                throw new Exception(
                    $"Could not create integration test user: {errors}");
            }
        }

        CustomerUserId = customerUser.Id;

        // ============================================
        // Create / Get Customer Profile
        // ============================================

        var customerProfile =
            await context.CustomerProfiles
                .FirstOrDefaultAsync(
                    c => c.UserId == customerUser.Id);

        if (customerProfile == null)
        {
            customerProfile = new CustomerProfile
            {
                UserId = customerUser.Id,
                FullName = "Integration Test Customer"
            };

            context.CustomerProfiles.Add(customerProfile);

            await context.SaveChangesAsync();
        }

        // ============================================
        // Create / Get Category
        // ============================================

        var category =
            await context.TicketCategories
                .FirstOrDefaultAsync(c => c.Code == "TEST");

        if (category == null)
        {
            category = new TicketCategory
            {
                Name = "Integration Test Category",
                Code = "TEST",
                Description = "Category used by integration tests",
                IsActive = true
            };

            context.TicketCategories.Add(category);

            await context.SaveChangesAsync();
        }

        CategoryId = category.Id;
    }
}