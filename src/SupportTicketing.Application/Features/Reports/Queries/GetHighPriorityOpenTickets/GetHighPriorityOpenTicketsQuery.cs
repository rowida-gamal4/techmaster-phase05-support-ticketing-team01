using MediatR;
using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.Queries.GetHighPriorityOpenTickets
{
    public class GetHighPriorityOpenTicketsQuery: IRequest<GetHighPriorityOpenTicketsResult>
    {
        public HighPriorityTicketRequestDTo Request { get; set; } = new();
    }
}