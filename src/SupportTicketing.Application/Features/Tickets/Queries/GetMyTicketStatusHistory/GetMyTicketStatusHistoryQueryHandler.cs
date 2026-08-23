using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketStatusHistory;

public class GetMyTicketStatusHistoryQueryHandler
	: IRequestHandler<GetMyTicketStatusHistoryQuery, GetMyTicketStatusHistoryResult>
{
	private readonly ICurrentUserService currentUserService;
	private readonly IApplicationDbContext dbContext;
	private readonly IValidator<GetMyTicketStatusHistoryQuery> validator;

	public GetMyTicketStatusHistoryQueryHandler(ICurrentUserService currentUserService,
		IApplicationDbContext dbContext, IValidator<GetMyTicketStatusHistoryQuery> validator)
	{
		this.currentUserService = currentUserService;
		this.dbContext = dbContext;
		this.validator = validator;
	}
	public async Task<GetMyTicketStatusHistoryResult> Handle(GetMyTicketStatusHistoryQuery request, CancellationToken cancellationToken)
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

		var ownsTicket = await dbContext.Tickets.AsNoTracking()
			.AnyAsync(t => t.Id == request.TicketId && t.Customer.UserId == currentUserId, cancellationToken);

		if(!ownsTicket)
			throw new NotFoundException("Ticket not found.");

		var history = await dbContext.TicketStatusHistories.AsNoTracking()
			.Where(t => t.TicketId == request.TicketId)
			.OrderBy(t => t.ChangedAt)
			.Select(t => new MyTicketStatusHistoryDto
			{
				Id = t.Id,
				OldStatus = t.OldStatus.ToString(),
				NewStatus = t.NewStatus.ToString(),
				ChangedAt = t.ChangedAt,
				Reason = t.Reason
			})
			.ToListAsync(cancellationToken);

		return new GetMyTicketStatusHistoryResult
		{
			TicketId = request.TicketId,
			History = history
		};
	}
}