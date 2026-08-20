using MediatR;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Common.Models;
using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand,  GeneralResponseDto<AuthResponseDto>>
{
    private readonly IIdentityService identityService;
   

    public RegisterCommandHandler(IIdentityService identityService )
    {
        this.identityService = identityService;
        
    }
    public async Task<GeneralResponseDto<AuthResponseDto>> Handle(RegisterCommand request,CancellationToken cancellationToken)
    {
        var result = await identityService.CreateUserAsync(request.Request.FullName,request.Request.Email,request.Request.Password,request.Request.Role,
            cancellationToken);

       return result ; 

    }
}