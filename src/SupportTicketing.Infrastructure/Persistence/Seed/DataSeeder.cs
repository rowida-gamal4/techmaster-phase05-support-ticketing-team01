using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Infrastructure.Persistence;

namespace SupportTicketing.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedCategoriesAsync(context);
        await SeedSlaPoliciesAsync(context);
    }

    private static async Task SeedCategoriesAsync(AppDbContext context)
    {
        if (await context.TicketCategories.AnyAsync())
            return;

        var categories = new List<TicketCategory>
        {
            new TicketCategory
            {
                Name = "Technical Support",
                Code = "T-1",
                Description = "Technical problems and system issues.",
                IsActive = true
            },

            new TicketCategory
            {
                Name = "Billing",
                Code = "B-1",
                Description = "Billing, invoices, and payment issues.",
                IsActive = true
            },

            new TicketCategory
            {
                Name = "Account",
                Code = "A-1",
                Description = "Account access and profile issues.",
                IsActive = true
            }
        };

        await context.TicketCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSlaPoliciesAsync(AppDbContext context)
    {
        if (await context.SlaPolicies.AnyAsync())
            return;

        var technicalCategory = await context.TicketCategories.FirstAsync(c => c.Code == "T-1");

        var billingCategory = await context.TicketCategories.FirstAsync(c => c.Code == "B-1");

        var accountCategory = await context.TicketCategories.FirstAsync(c => c.Code == "A-1");

        var policies = new List<SlaPolicy>
    {
       
        new SlaPolicy
        {
            CategoryId = technicalCategory.Id,
            Priority = TicketPriority.Low,
            ResponseTimeMin = 480,
            ResolutionTimeMin = 2880,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = technicalCategory.Id,
            Priority = TicketPriority.Medium,
            ResponseTimeMin = 240,
            ResolutionTimeMin = 1440,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = technicalCategory.Id,
            Priority = TicketPriority.High,
            ResponseTimeMin = 120,
            ResolutionTimeMin = 480,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = technicalCategory.Id,
            Priority = TicketPriority.Critical,
            ResponseTimeMin = 30,
            ResolutionTimeMin = 120,
            IsActive = true
        },

        
        new SlaPolicy
        {
            CategoryId = billingCategory.Id,
            Priority = TicketPriority.Low,
            ResponseTimeMin = 480,
            ResolutionTimeMin = 2880,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = billingCategory.Id,
            Priority = TicketPriority.Medium,
            ResponseTimeMin = 240,
            ResolutionTimeMin = 1440,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = billingCategory.Id,
            Priority = TicketPriority.High,
            ResponseTimeMin = 120,
            ResolutionTimeMin = 480,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = billingCategory.Id,
            Priority = TicketPriority.Critical,
            ResponseTimeMin = 30,
            ResolutionTimeMin = 120,
            IsActive = true
        },

       
        new SlaPolicy
        {
            CategoryId = accountCategory.Id,
            Priority = TicketPriority.Low,
            ResponseTimeMin = 480,
            ResolutionTimeMin = 2880,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = accountCategory.Id,
            Priority = TicketPriority.Medium,
            ResponseTimeMin = 240,
            ResolutionTimeMin = 1440,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = accountCategory.Id,
            Priority = TicketPriority.High,
            ResponseTimeMin = 120,
            ResolutionTimeMin = 480,
            IsActive = true
        },

        new SlaPolicy
        {
            CategoryId = accountCategory.Id,
            Priority = TicketPriority.Critical,
            ResponseTimeMin = 30,
            ResolutionTimeMin = 120,
            IsActive = true
        }
    };

        await context.SlaPolicies.AddRangeAsync(policies);
        await context.SaveChangesAsync();
    }
}