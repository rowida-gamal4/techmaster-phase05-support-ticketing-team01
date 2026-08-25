using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Customer;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Comments.Commands.AddAgentPublicReply
{


    public class AddAgentPublicReplyCommandHandler : IRequestHandler<AddAgentPublicReplyCommand, AddAgentPublicReplyResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly IValidator<AddAgentPublicReplyCommand> validator;
        private readonly ILogger<AddAgentPublicReplyCommandHandler> logger;

        public AddAgentPublicReplyCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<AddAgentPublicReplyCommand> validator, ILogger<AddAgentPublicReplyCommandHandler> logger)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.validator = validator;
            this.logger = logger;
        }

        public async Task<AddAgentPublicReplyResult> Handle(
            AddAgentPublicReplyCommand request,
            CancellationToken cancellationToken)
        {

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required.");
            }

            var userId = currentUserService.UserId.Value;

            var agent = await dbContext.AgentProfiles.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
            if (agent is null)
            {
                throw new ForbiddenException("Agent profile was not found.");
            }

            var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);
            if (ticket is null)
            {
                throw new NotFoundException("Ticket was not found.");
            }
            var isAssigned = await dbContext.TicketAssignments.AnyAsync(a => a.TicketId == ticket.Id && a.AgentId == agent.Id, cancellationToken);

            if (!isAssigned)
            {
                throw new ForbiddenException("You are not assigned to this ticket.");
            }

            if (ticket.Status == TicketStatus.Closed)
            {
                throw new BusinessRuleException("Replies can not be added to a closed ticket.");
            }

            if (ticket.Status == TicketStatus.Cancelled)
            {
                throw new BusinessRuleException("Replies can not be added to a cancelled ticket.");
            }

            var comment = new TicketComment
            {
                TicketId = ticket.Id,
                AuthorUserId = userId,
                Content = request.Request.Content,
                Visibility = CommentVisibility.Public
            };

            dbContext.TicketComments.Add(comment);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Agent {UserId} added a public reply to ticket {TicketId}", userId, ticket.Id);

            return new AddAgentPublicReplyResult
            {
                Comment = new AddCommentResponseDto
                {
                    Id = comment.Id,
                    TicketId = comment.TicketId,
                    AuthorUserId = comment.AuthorUserId,
                    Content = comment.Content,
                    Visibility = comment.Visibility.ToString(),
                    CreatedAt = comment.CreatedAt
                }
            };
        }
    }
}