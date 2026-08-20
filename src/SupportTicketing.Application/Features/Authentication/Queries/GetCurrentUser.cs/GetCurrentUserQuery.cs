using MediatR;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.DTOs.Auth;

namespace SupportTicketing.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery
    : IRequest<GeneralResponseDto<AuthResponseDto>>;