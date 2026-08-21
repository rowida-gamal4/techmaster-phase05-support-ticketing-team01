
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Features.Tickets.Commands.CancelTicket;
using SupportTicketing.Application.Features.Tickets.Queries.GetMyCustomerTickets;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Customer)]
public class CustomersController : ControllerBase
{
    private readonly IMediator mediator;

    public CustomersController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("me/tickets")]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> GetMyTickets([FromQuery] GetMyTicketsRequestDto request, CancellationToken cancellationToken)
    {
        var query = new GetMyCustomerTicketsQuery(request);

        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }
    [HttpPatch("cancel/my-tickets/{ticketId}")]
    public async Task<IActionResult> CancelTicket(int ticketId,CancelTicketRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CancelTicketCommand(ticketId,request);

        var result = await mediator.Send(command,cancellationToken);

        return Ok(result);
    }
}