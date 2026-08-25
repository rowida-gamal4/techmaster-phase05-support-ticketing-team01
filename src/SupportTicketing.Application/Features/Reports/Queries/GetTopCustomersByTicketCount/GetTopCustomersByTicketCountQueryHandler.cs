using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Reports;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Reports.Queries.GetTopCustomersByTicketCount;

public class GetTopCustomersByTicketCountQueryHandler
    : IRequestHandler<
        GetTopCustomersByTicketCountQuery,
        GetTopCustomersByTicketCountResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;

    public GetTopCustomersByTicketCountQueryHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
    }
    public async Task<GetTopCustomersByTicketCountResult> Handle(GetTopCustomersByTicketCountQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new UnauthorizedAccessException("Authentication is required");
        }

        var currentUserId = currentUserService.UserId.Value;

        if (currentUserService.Role != Roles.Admin && currentUserService.Role != Roles.SupportLead)
        {
            throw new ForbiddenException("Only Admin or SupportLead can view customer reports");
        }

        if(request.PageNumber < 1)
            throw new BusinessRuleException("PageNumber must be greater than 0");

        if (request.PageSize < 1 || request.PageSize > 100)
            throw new BusinessRuleException("PageNumber must be between 1 and 100");


        var query = dbContext.CustomerProfiles.AsNoTracking().Select(c => new 
        {
            CustomerId = c.Id,
            CustomerName = c.FullName,
            TicketCount = dbContext.Tickets.Count(t => t.CustomerId == c.UserId)
        }).Where(x=>x.TicketCount > 0);

        var totalCount = await query.CountAsync(cancellationToken);

        var customers = await query.OrderByDescending(x=>x.TicketCount)
            .ThenBy(x=>x.CustomerName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new TopCustomerDto
            {
                CustomerId = x.CustomerId,
                CustomerName = x.CustomerName,
                TicketCount = x.TicketCount
            }).ToListAsync(cancellationToken);

        var totalPages =
            (int)Math.Ceiling(
                totalCount / (double)request.PageSize);

        return new GetTopCustomersByTicketCountResult
        {
            Customers = customers,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

    }
}