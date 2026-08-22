using SupportTicketing.Application.DTOs.Customer;

namespace SupportTicketing.Application.Features.Comments.Commands.AddAgentPublicReply
{

    public class AddAgentPublicReplyResult
    {
        public AddCommentResponseDto Comment { get; set; } = null!;
    }
}