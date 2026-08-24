namespace SupportTicketing.Application.DTOs.Reports
{
    public class TicketCategoryDistributionDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public int TicketCount { get; set; }
    }
}

