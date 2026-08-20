using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.Common;
using SupportTicketing.Application.Common.Models;
using SupportTicketing.Application.Features.Auth.Commands.Login;
using SupportTicketing.Application.Features.Auth.Commands.Register;
using SupportTicketing.Application.Features.Auth.Queries.GetCurrentUser;

namespace SupportTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator mediator;

    public AuthController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (!result.Success)
           return HandleError(result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
          

        return Ok(result);
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrentUserQuery(),cancellationToken);

        return Ok(result);
    }

     private IActionResult HandleError<T>(GeneralResponseDto<T> result)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result),
                ErrorType.Conflict => Conflict(result),
                ErrorType.BadRequest => BadRequest(result),
                _ => BadRequest(result)
            };
        }
}