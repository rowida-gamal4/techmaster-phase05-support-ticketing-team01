using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketCategoryDistribution
{
    public class GetTicketCategoryDistributionResult
    {
        public List<TicketCategoryDistributionDto> Categories { get; set; } = new();
    }
}

