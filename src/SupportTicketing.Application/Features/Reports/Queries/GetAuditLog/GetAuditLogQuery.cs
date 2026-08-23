using MediatR;
using SupportTicketing.Application.DTOs.Reports;

namespace SupportTicketing.Application.Features.Reports.GetAuditLog
{
    public record GetAuditLogQuery(GetAuditLogRequestDto Request) : IRequest<GetAuditLogResult>;
}