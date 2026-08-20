using MediatR;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Common.Models;
using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler: IRequestHandler<GetCurrentUserQuery, GeneralResponseDto<AuthResponseDto>>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IIdentityService identityService;

    public GetCurrentUserQueryHandler(ICurrentUserService currentUserService, IIdentityService identityService)
    {
        this.currentUserService = currentUserService;
        this.identityService = identityService;
    }

    public async Task<GeneralResponseDto<AuthResponseDto>> Handle(GetCurrentUserQuery request,CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
           return new GeneralResponseDto<AuthResponseDto>
            {
                Success = false,
                Message = "User is not authenticated.",
                ErrorType = ErrorType.Unauthorized,
                Errors = new List<string>
                {
                    "Authentication is required."
                }
            };
        }

        var user = await identityService.GetUserByIdAsync(currentUserService.UserId.Value, cancellationToken);

        if (user is null)
        {
           return new GeneralResponseDto<AuthResponseDto>
            {
                Success = false,
                Message = "Current user was not found.",
                ErrorType = ErrorType.NotFound,
                Errors = new List<string>
                {
                    "The current user does not exist or is inactive."
                }
            };
        }

       return new GeneralResponseDto<AuthResponseDto>
        {
            Success = true,
            Message = "Current user retrieved successfully.",
            Data = new AuthResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            }
        };
    }

  
}