using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Features.Auth.Commands.Login;

public class LoginResult
{
    public AuthResponseDto User { get; set; } = null!;
}