using SupportTicketing.Application.Common.Models;
using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<GeneralResponseDto<AuthResponseDto>> CreateUserAsync(string fullName,string email,string password,string role,CancellationToken cancellationToken);

    Task<IdentityUserResult?>ValidateCredentialsAsync(string email,string password,CancellationToken cancellationToken);

    Task<IdentityUserResult?> GetUserByIdAsync(
    int userId,
    CancellationToken cancellationToken);
}