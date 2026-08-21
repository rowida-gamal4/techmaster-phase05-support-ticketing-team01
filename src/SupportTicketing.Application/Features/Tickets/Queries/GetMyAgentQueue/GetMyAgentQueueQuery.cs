using MediatR;
using SupportTicketing.Application.DTOs.TicketAssignment;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyAgentQueue;

public record GetMyAgentQueueQuery(int PageNumber = 1, int PageSize = 20) : IRequest<GetMyAgentQueueResult>;