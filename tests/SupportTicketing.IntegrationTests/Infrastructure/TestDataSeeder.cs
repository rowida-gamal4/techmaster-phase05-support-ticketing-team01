using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Infrastructure.Persistence;

namespace SupportTicketing.IntegrationTests.Infrastructure;

public static class TestDataSeeder
{
    public static int CustomerUserId { get; private set; }
    public static int OtherCustomerUserId { get; private set; }
    public static int CategoryId { get; private set; }
    public static int OtherCustomerTicketId { get; private set; }
    public static int AgentUserId { get; private set; }
    public static int AgentProfileId { get; private set; }


    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        await context.Database.EnsureCreatedAsync();

        // ============================================
        // CUSTOMER A
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
        // CUSTOMER A PROFILE
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
        // CUSTOMER B
        // ============================================

        var otherCustomerUser =
            await userManager.FindByEmailAsync(
                "integration.other.customer@test.com");

        if (otherCustomerUser == null)
        {
            otherCustomerUser = new ApplicationUser
            {
                UserName = "integration.other.customer@test.com",
                Email = "integration.other.customer@test.com",
                FullName = "Other Customer",
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(
                otherCustomerUser,
                "Password123!");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e =>
                        $"{e.Code}: {e.Description}"));

                throw new Exception(
                    $"Failed to create other customer: {errors}");
            }
        }

        OtherCustomerUserId = otherCustomerUser.Id;

        // ============================================
        // CUSTOMER B PROFILE
        // ============================================

        var otherCustomerProfile =
            await context.CustomerProfiles
                .FirstOrDefaultAsync(
                    c => c.UserId == otherCustomerUser.Id);

        if (otherCustomerProfile == null)
        {
            otherCustomerProfile = new CustomerProfile
            {
                UserId = otherCustomerUser.Id,
                FullName = "Other Integration Customer"
            };

            context.CustomerProfiles.Add(otherCustomerProfile);

            await context.SaveChangesAsync();
        }

        // ============================================
        // CATEGORY
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

        // ============================================
        // CUSTOMER B TICKET
        // ============================================

        var otherTicket =
            await context.Tickets
                .FirstOrDefaultAsync(
                    t =>
                        t.CustomerId == otherCustomerProfile.Id &&
                        t.Title == "Other Customer Integration Ticket");

        if (otherTicket == null)
        {
            otherTicket = new Ticket
            {
                CustomerId = otherCustomerProfile.Id,
                CategoryId = category.Id,
                Title = "Other Customer Integration Ticket",
                Description = "Ticket owned by Customer B",
                Status = TicketStatus.New,
                Priority = TicketPriority.Low
            };

            otherTicket.SetCreatedAt();

            context.Tickets.Add(otherTicket);

            await context.SaveChangesAsync();
        }

        OtherCustomerTicketId = otherTicket.Id;

        // ============================================
        // AGENT
        // ============================================

        var agentUser = await userManager.FindByEmailAsync("integration.agent@test.com");

        if (agentUser == null)
        {
            agentUser = new ApplicationUser
            {
                UserName = "integration.agent@test.com",
                Email = "integration.agent@test.com",
                FullName = "Integration Test Agent",
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(
                agentUser,
                "TestPassword123!");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e =>
                        $"{e.Code}: {e.Description}"));

                throw new Exception(
                    $"Could not create integration test agent: {errors}");
            }
        }

        AgentUserId = agentUser.Id;
        // ============================================
        // AGENT PROFILE
        // ============================================

        var agentProfile =
            await context.AgentProfiles
                .FirstOrDefaultAsync(
                    a => a.UserId == agentUser.Id);

        if (agentProfile == null)
        {
            agentProfile = new AgentProfile
            {
                UserId = agentUser.Id,
                FullName = "Integration Test Agent",
                IsActive = true
            };

            context.AgentProfiles.Add(agentProfile);

            await context.SaveChangesAsync();
        }

        AgentProfileId = agentProfile.Id;
    }
}