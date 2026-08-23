using MediatR;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Features.Reports.Queries;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTicketsByStatusReport
{
    public record GetTicketsByStatusQuery( GetTicketsByStatusRequestDto Request): IRequest<GetTicketsByStatusResult>;
}

