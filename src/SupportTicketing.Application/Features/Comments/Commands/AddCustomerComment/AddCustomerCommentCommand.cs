using MediatR;
using SupportTicketing.Application.DTOs.Customer;

namespace SupportTicketing.Application.Features.Comments.Commands.AddCustomerComment
{
    public record AddCustomerCommentCommand (int TicketId,AddCommentRequestDto Request) : IRequest<AddCustomerCommentResult>;
}