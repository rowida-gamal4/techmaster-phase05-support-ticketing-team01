

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Infrastructure.Persistence;
using SupportTicketing.Infrastructure.Services;
using SupportTicketing.Services;

namespace SupportTicketing.Infrastructure
{
    public static class InfrastructureServicesRegisterations
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = false;
            }).AddRoles<IdentityRole<int>>().AddEntityFrameworkStores<AppDbContext>();


            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITokenService, TokenServices>();
            //services.AddScoped<IIdentityService, IdentityService>();


            return services;
        }
    }
}