using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Infrastructure.Persistence;
using SupportTicketing.IntegrationTests.Infrastructure;
using Xunit;

namespace SupportTicketing.IntegrationTests.Tickets;

public class TicketStatusTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory factory;

    public TicketStatusTests(
        CustomWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task InvalidStatusTransition_NewToClosed_ReturnsBadRequest()
    {
        // Arrange
        await SeedDatabaseAsync();

        var client = factory.CreateClient();

        // Authenticate as the agent
        client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            TestDataSeeder.AgentUserId.ToString());

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            "SupportAgent");

        var ticketId =
            TestDataSeeder.CustomerTicketId;

        // Ticket must be in New status
        Assert.Equal(
            TicketStatus.New,
            await GetTicketStatusAsync(ticketId));

        // Try invalid transition: New -> Closed
        var request = new
        {
            status = TicketStatus.Closed
        };

        // Act
        var response = await client.PatchAsJsonAsync(
            $"/api/tickets/{ticketId}/status",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope =
            factory.Services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

        await TestDataSeeder.SeedAsync(
            context,
            userManager);

        // --------------------------------------------
        // Make the agent authorized for this ticket
        // --------------------------------------------

        var existingAssignment =
            await context.TicketAssignments
                .FirstOrDefaultAsync(a =>
                    a.TicketId == TestDataSeeder.CustomerTicketId &&
                    a.AgentId == TestDataSeeder.AgentProfileId &&
                    a.IsActive);

        if (existingAssignment == null)
        {
            var assignment = new TicketAssignment
            {
                TicketId = TestDataSeeder.CustomerTicketId,
                AgentId = TestDataSeeder.AgentProfileId,
                TeamId = TestDataSeeder.SupportTeamId,
                AssignedByUserId = TestDataSeeder.AgentUserId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            context.TicketAssignments.Add(assignment);

            await context.SaveChangesAsync();
        }
    }

    private async Task<TicketStatus> GetTicketStatusAsync(
        int ticketId)
    {
        using var scope =
            factory.Services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        return await context.Tickets
            .Where(t => t.Id == ticketId)
            .Select(t => t.Status)
            .SingleAsync();
    }
}