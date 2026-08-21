using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.Features.Tickets.Commands.CreateTicket;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.API.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize(Roles = Roles.Customer)]
public class TicketsController : ControllerBase
{
    private readonly IMediator mediator;

    public TicketsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> CreateTicket( CreateTicketCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}