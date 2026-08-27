using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Infrastructure.Persistence;
using SupportTicketing.IntegrationTests.Infrastructure;
using Xunit;

namespace SupportTicketing.IntegrationTests.Tickets;

public class AgentTicketAccessTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory factory;

    public AgentTicketAccessTests(
        CustomWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task AgentCannotAccessUnassignedTicket_ReturnsForbiddenOrNotFound()
    {
        // Arrange
        await SeedDatabaseAsync();

        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            TestDataSeeder.AgentUserId.ToString());

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            "SupportAgent");

        var ticketId = TestDataSeeder.OtherCustomerTicketId;

        // Act
        var response = await client.GetAsync(
            $"/api/tickets/{ticketId}");

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.NotFound);
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        await TestDataSeeder.SeedAsync(
            context,
            userManager);
    }
}