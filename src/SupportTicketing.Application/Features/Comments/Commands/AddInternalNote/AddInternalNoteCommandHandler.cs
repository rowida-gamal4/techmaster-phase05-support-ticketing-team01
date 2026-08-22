using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Comments.Commands.AddInternalNote;

public class AddInternalNoteCommandHandler : IRequestHandler<AddInternalNoteCommand, AddInternalNoteResult>
{
    private readonly ICurrentUserService currentUserService;
    private readonly IApplicationDbContext dbContext;
    private readonly IValidator<AddInternalNoteCommand> validator;

    public AddInternalNoteCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<AddInternalNoteCommand> validator)
    {
        this.currentUserService = currentUserService;
        this.dbContext = dbContext;
        this.validator = validator;
    }
    public async Task<AddInternalNoteResult> Handle(AddInternalNoteCommand request, CancellationToken cancellationToken)
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

        if (currentUserService.Role != Roles.SupportAgent && currentUserService.Role != Roles.SupportLead)
        {
            throw new ForbiddenException("Only support staf can add internal notes ");
        }

        var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket is null)
            throw new NotFoundException("Ticket not found");

        if (ticket.Status == TicketStatus.Closed)
        {
            throw new BusinessRuleException("Comments can not be added to a closed ticket.");
        }

        var comment = new TicketComment
        {
            TicketId = ticket.Id,
            AuthorUserId = currentUserId,
            Content = request.Request.Content,
            Visibility = CommentVisibility.Internal,
        };

        await dbContext.TicketComments.AddAsync(comment, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddInternalNoteResult
        {
            TicketId = ticket.Id,
            CommentId = comment.Id,
            Message = "Internal note added successfully"
        };
    }
}