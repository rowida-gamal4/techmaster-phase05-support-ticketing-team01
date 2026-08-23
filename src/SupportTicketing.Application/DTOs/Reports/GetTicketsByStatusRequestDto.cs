namespace SupportTicketing.Application.DTOs.Reports
{
    public class GetTicketsByStatusRequestDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}