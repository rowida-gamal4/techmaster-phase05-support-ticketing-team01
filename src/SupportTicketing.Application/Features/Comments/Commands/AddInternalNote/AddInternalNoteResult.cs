namespace SupportTicketing.Application.Features.Comments.Commands.AddInternalNote;

public class AddInternalNoteResult
{
    public int TicketId { get; set; }
    public int CommentId {  get; set; }
    public string Message { get; set; }
}