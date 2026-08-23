using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicket;

public class GetMyTicketQueryHandler : IRequestHandler<GetMyTicketQuery, GetMyTicketResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<GetMyTicketQuery> validator;

    public GetMyTicketQueryHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<GetMyTicketQuery> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<GetMyTicketResult> Handle(GetMyTicketQuery request, CancellationToken cancellationToken)
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

        var ticket = await dbContext.Tickets.AsNoTracking().Where(t => t.Id == request.TicketId)
            .Where(t => t.Customer.UserId == currentUserId).Select(t => new MyTicketDetailsDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                CreatedAt = t.CreatedAt,
                StartedAt = t.StartedAt,
                ResolvedAt = t.ResolvedAt,
                ClosedAt = t.ClosedAt,
                CancelledAt = t.CancelledAt,
                CancellationReason = t.CancellationReason,
            }).FirstOrDefaultAsync(cancellationToken);

        if (ticket is null)
            throw new NotFoundException("Ticket not found");

        return new GetMyTicketResult
        {
            Ticket = ticket
        };

    }
}