using SupportTicketing.Application.DTOs.Customer;
using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Comments.Commands.AddCustomerComment
{
    public class AddCustomerCommentResult
    {
        public AddCommentResponseDto Comment { get; set; } = null!;
    }
}