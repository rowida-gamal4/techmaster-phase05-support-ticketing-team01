using MediatR;
using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Commands.AddTicketAttachmentMetadata
{
    public record AddTicketAttachmentMetadataCommand(int TicketId, AddTicketAttachmentMetadataRequestDto Request) : IRequest<AddTicketAttachmentMetadataResult>
    {

    }
}