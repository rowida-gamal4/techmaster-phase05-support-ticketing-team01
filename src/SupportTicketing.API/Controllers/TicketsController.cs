using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.Features.Tickets.Commands.CreateTicket;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Application.Features.Tickets.Commands.AssignTicket;
using SupportTicketing.Application.DTOs.TicketAssignment;


namespace SupportTicketing.API.Controllers;

[ApiController]
[Route("api/tickets")]
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
    [HttpPost("{ticketId}/assign")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SupportLead)]
    public async Task<IActionResult> AssignTicket(int ticketId, AssignTicketRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AssignTicketCommand(ticketId, request);
        var result = await mediator.Send(command,cancellationToken);
        return Ok(result);
    }

}