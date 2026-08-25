using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetUnassignedTickets;

public class GetUnassignedTicketsQueryHandler
    : IRequestHandler<GetUnassignedTicketsQuery, GetUnassignedTicketsResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<GetUnassignedTicketsQuery> validator;

    public GetUnassignedTicketsQueryHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<GetUnassignedTicketsQuery> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<GetUnassignedTicketsResult> Handle(GetUnassignedTicketsQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new UnauthorizedAccessException("Authentication is required");
        }

        var currentUserId = currentUserService.UserId.Value;

        if (currentUserService.Role != Roles.Admin && currentUserService.Role != Roles.SupportLead)
        {
            throw new ForbiddenException("Only Admin or SupportLead can view unassigned tickets");
        }

        var query = dbContext.Tickets.AsNoTracking().Where(t => t.Status != TicketStatus.Closed && t.Status != TicketStatus.Cancelled && !t.Assignments.Any(a => a.IsActive));

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<TicketStatus>(request.Status, true, out var status))
                query = query.Where(t => t.Status == status);
            else
                throw new BusinessRuleException($"Invalid ticket status : {request.Status}");
        }

        if (!string.IsNullOrWhiteSpace(request.Priority))
        {
            if (Enum.TryParse<TicketPriority>(request.Priority, true, out var priority))
                query = query.Where(t => t.Priority == priority);
            else
                throw new BusinessRuleException($"Invalid ticket priority : {request.Priority}");
        }

        var totalCount = await query.CountAsync(cancellationToken);

        if (request.SortBy.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
            query = query.OrderByDescending(t => t.CreatedAt);
        else
            query = query.OrderByDescending(t => t.Priority).ThenBy(t => t.CreatedAt);

            var tickets = await query
                .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).Select(t => new UnassignedTicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    CategoryId = t.CategoryId,
                    CreatedAt = t.CreatedAt
                }).ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new GetUnassignedTicketsResult
        {
            Tickets = tickets,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
        };

    }
}