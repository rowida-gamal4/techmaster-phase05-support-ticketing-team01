using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Infrastructure.Persistence;
using SupportTicketing.IntegrationTests.Infrastructure;
using Xunit;
using System.Net.Http.Json;

namespace SupportTicketing.IntegrationTests.Comments;

public class InternalNoteTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory factory;

    public InternalNoteTests(
        CustomWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Customer_CannotAddInternalNote_ReturnsForbidden()
    {
        // Arrange
        await SeedDatabaseAsync();

        var client = factory.CreateClient();

        // Authenticate as Customer A
        client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            TestDataSeeder.CustomerUserId.ToString());

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            "Customer");

        // Ticket belongs to Customer A
        var ticketId = TestDataSeeder.CustomerTicketId;

        var request = new
        {
            content = "This should not be allowed as an internal note."
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"/api/tickets/{ticketId}/internal-notes",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = factory.Services.CreateScope();

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