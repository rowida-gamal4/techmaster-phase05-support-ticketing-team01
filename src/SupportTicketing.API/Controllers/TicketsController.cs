using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.Features.Tickets.Commands.CreateTicket;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Application.Features.Tickets.Commands.AssignTicket;
using SupportTicketing.Application.DTOs.TicketAssignment;
using SupportTicketing.Application.DTOs.TicketTriage;
using SupportTicketing.Application.Features.Tickets.Commands.ReassignTicket;
using SupportTicketing.Application.Features.Tickets.Queries.GetMyAgentQueue;
using SupportTicketing.Application.Features.Tickets.Commands.SetTicketPriority;
using SupportTicketing.Application.Features.Tickets.Commands.StartTicket;
using SupportTicketing.Application.Features.Tickets.Commands.ResolveTicket;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Features.Tickets.Commands.ChangeTicketStatus;
using SupportTicketing.Application.Features.Comments.Commands.AddInternalNote;
using SupportTicketing.Application.DTOs.TicketComment;
using SupportTicketing.Application.Features.Tickets.Commands.AddTicketAttachmentMetadata;
using SupportTicketing.Application.Features.Tickets.Queries.GetTicketAttachments;

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
    public async Task<IActionResult> CreateTicket(CreateTicketCommand command,
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
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    [HttpPost("{ticketId}/reassign")]
    [Authorize(Roles = Roles.SupportLead)]
    public async Task<IActionResult> ReassignTicket(int ticketId, ReassignTicketRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ReassignTicketCommand(ticketId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    [HttpGet("my-queue")]
    [Authorize(Roles = Roles.SupportAgent)]
    public async Task<IActionResult> GetMyQueue([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetMyAgentQueueQuery(pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpPatch("{ticketId}/priority")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SupportLead)]
    public async Task<IActionResult> SetPriority(int ticketId, SetPriorityRequestDto request, CancellationToken cancellationToken)
    {
        var command = new SetPriorityCommand(ticketId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    [HttpPut("{ticketId}/start")]
    [Authorize(Roles = Roles.SupportAgent)]
    public async Task<IActionResult> StartTicket(int ticketId, CancellationToken cancellationToken)
    {
        var command = new StartTicketCommand(ticketId);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    [HttpPut("{ticketId}/resolve")]
    [Authorize(Roles = Roles.SupportAgent)]
    public async Task<IActionResult> ResolveTicket(int ticketId, ResolveTicketRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ResolveTicketCommand(ticketId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }


    [HttpPatch("{id}/status")]
    [Authorize(Roles = Roles.Customer + "," + Roles.SupportLead)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTicketStatusRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateTicketStatusCommand(id, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    [HttpPost("{ticketId}/internal-notes")]
    [Authorize(Roles = Roles.SupportAgent + "," + Roles.SupportLead)]
    public async Task<IActionResult> AddInternalNote(int ticketId, AddInternalNoteRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AddInternalNoteCommand(ticketId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{ticketId}/attachments")]
    public async Task<IActionResult> AddAttachmentMetadata(int ticketId, [FromBody] AddTicketAttachmentMetadataRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AddTicketAttachmentMetadataCommand(ticketId, request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("attachments")]
    public async Task<IActionResult> GetAttachments([FromQuery] GetAttachmentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new GetTicketAttachmentsQuery(request);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

}
