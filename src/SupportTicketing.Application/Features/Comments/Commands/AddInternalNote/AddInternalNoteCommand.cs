using MediatR;
using SupportTicketing.Application.DTOs.TicketComment;

namespace SupportTicketing.Application.Features.Comments.Commands.AddInternalNote;

public record AddInternalNoteCommand(
    int TicketId,
    AddInternalNoteRequestDto Request
) : IRequest<AddInternalNoteResult>;