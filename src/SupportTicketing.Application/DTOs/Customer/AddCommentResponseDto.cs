namespace SupportTicketing.Application.DTOs.Customer
{
    public class AddCommentResponseDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int AuthorUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
