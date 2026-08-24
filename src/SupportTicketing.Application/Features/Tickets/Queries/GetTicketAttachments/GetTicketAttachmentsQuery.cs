using MediatR;
using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetTicketAttachments
{
    public record GetTicketAttachmentsQuery(GetAttachmentRequestDto Request) : IRequest<GetTicketAttachmentsResult>
    {

    }
}
