using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Infrastructure.Persistence;
using SupportTicketing.IntegrationTests.Infrastructure;
using Xunit;

namespace SupportTicketing.IntegrationTests.Tickets;

public class InactiveAgentAssignmentTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory factory;

    public InactiveAgentAssignmentTests(
        CustomWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task CannotAssignInactiveAgent_ReturnsForbiddenOrBadRequest()
    {
        // Arrange
        await SeedDatabaseAsync();

        var client = factory.CreateClient();

        // Authenticate as a Support Lead
        client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            TestDataSeeder.CustomerUserId.ToString());

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            "SupportLead");

        var ticketId =
            TestDataSeeder.OtherCustomerTicketId;

        var inactiveAgentId =
            TestDataSeeder.InactiveAgentProfileId;

        var request = new
        {
            agentId = inactiveAgentId
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"/api/tickets/{ticketId}/assign",
            request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Expected Forbidden or BadRequest, but received {response.StatusCode}");
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
    }
}