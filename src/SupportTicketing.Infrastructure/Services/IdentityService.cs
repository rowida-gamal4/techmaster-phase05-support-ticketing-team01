using System.Data;
using Microsoft.AspNetCore.Identity;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Common.Models;
using SupportTicketing.Application.DTOs.Auth;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Infrastructure.Persistence;


namespace SupportTicketing.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly AppDbContext dbContext;

    public IdentityService(UserManager<ApplicationUser> userManager,AppDbContext dbContext)
    {
        this.userManager = userManager;
        this.dbContext = dbContext;
    }

    public async Task<GeneralResponseDto<AuthResponseDto>> CreateUserAsync(string fullName,string email,string password,string role,CancellationToken cancellationToken)
    {
        var userExist = await userManager.FindByEmailAsync(email);

        if (userExist is not null)
        {
            return new GeneralResponseDto<AuthResponseDto>
            {
                Success = false,
                Message = "Email is already registered.",
                ErrorType = ErrorType.Conflict,
                Errors = new List<string>
                {
                    "Email is already registered."
                }
            };
        }

        var user = new ApplicationUser
        {
            FullName = fullName,
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToList();

           return new GeneralResponseDto<AuthResponseDto>
            {
                Success = false,
                Message = "User registration failed.",
                ErrorType = ErrorType.Validation,
                Errors = errors
            };
        }

        var roleResult = await userManager.AddToRoleAsync(user,role);

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            var errors = createResult.Errors.Select(e => e.Description).ToList();

           return new GeneralResponseDto<AuthResponseDto>
            {
                Success = false,
                Message = "User registration failed.",
                ErrorType = ErrorType.BadRequest,
                Errors = errors
            };
        }

        if (role == Roles.Customer)
        {
            var customerProfile = new CustomerProfile
            {
                UserId = user.Id,
                FullName = fullName,
            
            };

            dbContext.CustomerProfiles.Add(customerProfile);
        }
        else if (role == Roles.SupportAgent || role == Roles.SupportLead)
        {
            var agentProfile = new AgentProfile
            {
                UserId = user.Id,
                IsActive = true,
                FullName = fullName,
            };

            dbContext.AgentProfiles.Add(agentProfile);
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        return new GeneralResponseDto<AuthResponseDto>
        {
            Success = true,
            Message = "User registered successfully.",
            Data = new AuthResponseDto
            {
                FullName = user.FullName,
                Email = user.Email!,
                Role = role
            }
        };
    }

    public async Task<IdentityUserResult?> ValidateCredentialsAsync(string email,string password,CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !user.IsActive)
            return null;

        var validPassword = await userManager.CheckPasswordAsync( user, password);

        if (!validPassword)
            return null;

        var roles = await userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault();

        if (role is null)
            return null;

        return new IdentityUserResult
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = role
        };
    }

    public async Task<IdentityUserResult?> GetUserByIdAsync(int userId,CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null || !user.IsActive)
            return null;

        var roles = await userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault();

        if (role is null)
            return null;

        return new IdentityUserResult
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = role
        };
    }
}