using MediatR;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(
    RegisterRequestDto Request
    ) : IRequest< GeneralResponseDto<AuthResponseDto>>;
}