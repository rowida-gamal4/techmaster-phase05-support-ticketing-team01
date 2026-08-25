using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketing.Infrastructure.Persistence;
using SupportTicketing.IntegrationTests.Infrastructure;
using Xunit;
using Microsoft.AspNetCore.Identity;
using SupportTicketing.Domain.Entities;

namespace SupportTicketing.IntegrationTests.Tickets;

public class CreateTicketTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory factory;

    public CreateTicketTests(
        CustomWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task CreateTicket_WithValidCustomerAndData_ReturnsSuccess()
    {
        // Arrange
        await SeedDatabaseAsync();

        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            TestDataSeeder.CustomerUserId.ToString());

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            "Customer");

        var request = new
        {
            request = new
            {
                categoryId = TestDataSeeder.CategoryId,
                title = "Integration test ticket",
                description = "Created by integration test"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/tickets",
            request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Created);
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