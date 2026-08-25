using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketing.API;

namespace SupportTicketing.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory
	: WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(
		Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = "Test";
				options.DefaultChallengeScheme = "Test";
			})
			.AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
				"Test",
				options =>
				{
				});
		});
	}
}