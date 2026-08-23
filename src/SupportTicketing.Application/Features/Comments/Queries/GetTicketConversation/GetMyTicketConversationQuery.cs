using MediatR;

namespace SupportTicketing.Application.Features.Comments.Queries.GetMyTicketConversation;

public record GetMyTicketConversationQuery(
    int TicketId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<GetMyTicketConversationResult>;