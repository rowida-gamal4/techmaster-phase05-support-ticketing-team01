namespace SupportTicketing.Application.DTOs.Reports;

public class TopCustomerDto
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public int TicketCount { get; set; }
}