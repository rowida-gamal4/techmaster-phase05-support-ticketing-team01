using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketHistory;

public class GetMyTicketHistoryQueryHandler : IRequestHandler<GetMyTicketHistoryQuery, GetMyTicketHistoryResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<GetMyTicketHistoryQuery> validator;

    public GetMyTicketHistoryQueryHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<GetMyTicketHistoryQuery> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<GetMyTicketHistoryResult> Handle(GetMyTicketHistoryQuery request, CancellationToken cancellationToken)
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

        var query = dbContext.Tickets.AsNoTracking().Where(t => t.Customer.UserId == currentUserId);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<SupportTicketing.Domain.Enums.TicketStatus>(request.Status, true, out var status))
                query = query.Where(t => t.Status == status);
            else
                throw new BusinessRuleException($"Invalid ticket status : {request.Status}");
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(t => t.Title.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var tickets = await query.OrderByDescending(t=>t.CreatedAt)
            .Skip((request.PageNumber -1) * request.PageSize).Take(request.PageSize).Select(t=>new MyTicketHistoryItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                CategoryId = t.CategoryId,
                CreatedAt = t.CreatedAt,
                StartedAt = t.StartedAt,
                ResolvedAt = t.ResolvedAt,
                ClosedAt = t.ClosedAt
            }).ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new GetMyTicketHistoryResult
        {
            Tickets = tickets,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
        };

    }
}