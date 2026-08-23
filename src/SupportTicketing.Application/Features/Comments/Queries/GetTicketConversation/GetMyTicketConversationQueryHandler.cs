using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.TicketComment;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Comments.Queries.GetMyTicketConversation;

public class GetMyTicketConversationQueryHandler : IRequestHandler<GetMyTicketConversationQuery, GetMyTicketConversationResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<GetMyTicketConversationQuery> validator;

    public GetMyTicketConversationQueryHandler(ICurrentUserService currentUserService,
        IApplicationDbContext dbContext, IValidator<GetMyTicketConversationQuery> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<GetMyTicketConversationResult> Handle(GetMyTicketConversationQuery request, CancellationToken cancellationToken)
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

        if (!ownsTicket)
            throw new NotFoundException("Ticket not found");

        var commentsQuery = dbContext.TicketComments.AsNoTracking()
            .Where(c => c.TicketId == request.TicketId)
            .Where(c => c.Visibility == CommentVisibility.Public);

        var totalCount = await commentsQuery.CountAsync(cancellationToken);

        var comments = await commentsQuery.OrderBy(c => c.CreatedAt).Select(c => new MyTicketCommentDto
        {
            Id = c.Id,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
        }).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);

        return new GetMyTicketConversationResult
        {
            TicketId = request.TicketId,
            Comments = comments,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}