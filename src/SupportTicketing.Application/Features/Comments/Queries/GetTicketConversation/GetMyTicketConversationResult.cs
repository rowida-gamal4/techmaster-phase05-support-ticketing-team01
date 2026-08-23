using SupportTicketing.Application.DTOs.TicketComment;

namespace SupportTicketing.Application.Features.Comments.Queries.GetMyTicketConversation;

public class GetMyTicketConversationResult
{
    public int TicketId { get; set; }
    public List<MyTicketCommentDto> Comments { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}