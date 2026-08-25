using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketing.Application.DTOs.Customer;
using SupportTicketing.Application.DTOs.Agent;
using SupportTicketing.Application.Features.Agent.Queries.GetMyTeamQueue;
using SupportTicketing.Application.Features.Agent.Queries.GetMyActiveTickets;
using SupportTicketing.Application.Features.Comments.Commands.AddAgentPublicReply;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly IMediator mediator;

        public AgentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("add-replay/ticket/{ticketId}")]
        [Authorize(Roles = Roles.SupportAgent)]
        public async Task<IActionResult> AddPublicReply(int ticketId, AddCommentRequestDto request, CancellationToken cancellationToken)
        {
            var command = new AddAgentPublicReplyCommand(ticketId, request);
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet("my-team/queue")]
        [Authorize(Roles = Roles.SupportLead)]
        public async Task<IActionResult> GetMyTeamQueue(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetMyTeamQueueQuery(), cancellationToken);
            return Ok(result);
        }
        [HttpGet("my-active")]
        [Authorize(Roles = Roles.SupportAgent)]
        public async Task<IActionResult> GetMyActiveTickets([FromQuery] string sortBy = "priority",CancellationToken cancellationToken = default)
        {
            var query = new GetMyActiveTicketsQuery(sortBy);
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

    }
}

