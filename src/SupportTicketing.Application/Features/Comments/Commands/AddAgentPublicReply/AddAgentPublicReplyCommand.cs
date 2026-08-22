using MediatR;
using SupportTicketing.Application.DTOs.Customer;

namespace SupportTicketing.Application.Features.Comments.Commands.AddAgentPublicReply;

public record AddAgentPublicReplyCommand(int TicketId,AddCommentRequestDto Request) : IRequest<AddAgentPublicReplyResult>;