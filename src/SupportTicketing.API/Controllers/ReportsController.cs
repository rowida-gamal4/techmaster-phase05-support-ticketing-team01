using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.Features.Reports.Queries.GetAgentWorkloadReport;
using SupportTicketing.Domain.Enums;

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
    public async Task<IActionResult> GetAgentWorkload( CancellationToken cancellationToken)
    {
        var query = new GetAgentWorkloadQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}