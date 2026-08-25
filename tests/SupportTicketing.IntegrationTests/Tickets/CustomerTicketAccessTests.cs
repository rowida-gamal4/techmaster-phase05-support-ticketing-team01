using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Infrastructure.Persistence;
using SupportTicketing.IntegrationTests.Infrastructure;
using Xunit;

namespace SupportTicketing.IntegrationTests.Tickets;

public class CustomerTicketAccessTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory factory;

    public CustomerTicketAccessTests(
        CustomWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Customer_CannotAccessAnotherCustomersTicket_ReturnsNotFound()
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

        // This ticket belongs to Customer B
        var otherCustomerTicketId =
            TestDataSeeder.OtherCustomerTicketId;

        // Act
        var response = await client.GetAsync(
            $"/api/Customers/{otherCustomerTicketId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
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
    }
}