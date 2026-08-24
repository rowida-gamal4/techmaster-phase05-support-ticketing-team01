using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Features.Reports.GetAuditLog;
using SupportTicketing.Application.Features.Reports.GetResolutionTimeReport;
using SupportTicketing.Application.Features.Reports.Queries.GetAgentWorkloadReport;
using SupportTicketing.Application.Features.Reports.Queries.GetHighPriorityOpenTickets;
using SupportTicketing.Application.Features.Reports.Queries.GetTicketCategoryDistribution;
using SupportTicketing.Application.Features.Reports.Queries.GetTicketsByStatusReport;
using SupportTicketing.Application.Features.Sla.Queries.GetSlaRiskReport;
using SupportTicketing.Domain.Enums;
using SupportTicketing.Application.Features.Tickets.Queries.GetUnassignedTickets;


namespace SupportTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator mediator;

    public ReportsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("agent-workload")]
    [Authorize(Roles = Roles.SupportLead)]
    public async Task<IActionResult> GetAgentWorkload(CancellationToken cancellationToken)
    {
        var query = new GetAgentWorkloadQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("tickets/status-priority")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetTicketsByStatus([FromQuery] GetTicketsByStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new GetTicketsByStatusQuery(request);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("resolution-time")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SupportLead)]
    public async Task<IActionResult> GetResolutionTimeReport(CancellationToken cancellationToken)
    {
        var query = new GetResolutionTimeReportQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("audit")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAuditLog([FromQuery] GetAuditLogRequestDto request, CancellationToken cancellationToken)
    {
        var query = new GetAuditLogQuery(request);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("ticket-category-distribution")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SupportLead)]
    public async Task<IActionResult> GetTicketCategoryDistribution(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTicketCategoryDistributionQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("high-priority-open-tickets")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SupportLead)]
    public async Task<IActionResult> GetHighPriorityOpenTickets([FromQuery] HighPriorityTicketRequestDTo request, CancellationToken cancellationToken)
    {
        var query = new GetHighPriorityOpenTicketsQuery { Request = request };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("sla-risk")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SupportLead)]
    public async Task<IActionResult> GetApproachingSlaTickets(CancellationToken cancellationToken)
    {
        var query = new GetApproachingSlaTicketsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("unassigned")]
    [Authorize(Roles = Roles.Admin + "," + Roles.SupportLead)]
    public async Task<IActionResult> GetUnassignedTickets(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null,
        [FromQuery] string? priority = null, [FromQuery] string sortBy = "priority", CancellationToken cancellationToken = default)
    {
        var query = new GetUnassignedTicketsQuery(pageNumber, pageSize, status, priority, sortBy);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}