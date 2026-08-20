using MediatR;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    LoginRequestDto Request
) : IRequest<GeneralResponseDto<AuthResponseDto>>;