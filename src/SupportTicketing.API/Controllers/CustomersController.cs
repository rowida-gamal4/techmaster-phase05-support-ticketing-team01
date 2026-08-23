
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.DTOs.Customer;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Features.Comments.Commands.AddCustomerComment;
using SupportTicketing.Application.Features.Tickets.Commands.CancelTicket;
using SupportTicketing.Application.Features.Tickets.Queries.GetMyCustomerTickets;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Application.Features.Tickets.Queries.GetMyTicket;
using SupportTicketing.Application.Features.Comments.Queries.GetMyTicketConversation;
using SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketStatusHistory;
using SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketHistory;

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
    
    [HttpPost("add-comment/ticket/{ticketId}")]
    public async Task<IActionResult> AddComment(int ticketId,AddCommentRequestDto request,CancellationToken cancellationToken)
    {
        var command = new AddCustomerCommentCommand( ticketId,request);

        var result = await mediator.Send(command,cancellationToken);

        return Ok(result);
    }
    [HttpGet("{ticketId}")]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> GetMyTicket(int ticketId, CancellationToken cancellationToken)
    {
        var query = new GetMyTicketQuery(ticketId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{ticketId}/conversation")]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> GetMyTicketConversation(int ticketId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetMyTicketConversationQuery(ticketId,pageNumber,pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{ticketId}/status-history")]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> GetMyTicketStatusHistoy(int ticketId,CancellationToken cancellationToken)
    {
        var query = new GetMyTicketStatusHistoryQuery(ticketId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("my-history")]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> GetMyTicketHistory([FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? status = null,
    [FromQuery] string? search = null,
    CancellationToken cancellationToken = default)
    {
        var query = new GetMyTicketHistoryQuery(pageNumber, pageSize, status, search);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

