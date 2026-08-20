using MediatR;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Common.Models;
using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, GeneralResponseDto<AuthResponseDto>>
{
    private readonly IIdentityService identityService;
    private readonly ITokenService tokenService;

    public LoginCommandHandler( IIdentityService identityService,ITokenService tokenService)
    {
        this.identityService = identityService;
        this.tokenService = tokenService;
    }

    public async Task<GeneralResponseDto<AuthResponseDto>> Handle( LoginCommand request,CancellationToken cancellationToken)
    {
        var user = await identityService.ValidateCredentialsAsync( request.Request.Email,request.Request.Password,cancellationToken);

        if (user is null)
        {
           return new GeneralResponseDto<AuthResponseDto>
            {
                Success = false,
                Message = "Invalid email or password.",
                ErrorType = ErrorType.Unauthorized,
                Errors = new List<string>
                {
                    "Invalid email or password."
                }
            };
        }

        var token = tokenService.GenerateToken(user.UserId,user.Email,user.FullName, user.Role);

        return new GeneralResponseDto<AuthResponseDto>
        {
            Success = true,
            Message = "Login successful.",
            Data = new AuthResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token
            }
        };
    }
}