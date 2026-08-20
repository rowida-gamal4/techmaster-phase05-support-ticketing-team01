using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserResult
{
    public AuthResponseDto User { get; set; } = null!;
}