using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Customer;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Comments.Commands.AddCustomerComment
{
    public class AddCustomerCommentCommandHandler : IRequestHandler<AddCustomerCommentCommand, AddCustomerCommentResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly IValidator<AddCustomerCommentCommand> validator;

        public AddCustomerCommentCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IValidator<AddCustomerCommentCommand> validator)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.validator = validator;
        }
        public async Task<AddCustomerCommentResult> Handle(AddCustomerCommentCommand request, CancellationToken cancellationToken)
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

            var UserId = currentUserService.UserId.Value;

            var customer = await dbContext.CustomerProfiles.FirstOrDefaultAsync(c => c.UserId == UserId, cancellationToken);

            if (customer is null)
            {
                throw new ForbiddenException("Customer profile was not found.");
            }

            var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

            if (ticket is null)
            {
                throw new NotFoundException("Ticket was not found.");
            }

            if (ticket.CustomerId != customer.Id)
            {
                throw new ForbiddenException("This ticket does not belong to you.");
            }

            if (ticket.Status == TicketStatus.Closed)
            {
                throw new BusinessRuleException("Comments can not be added to a closed ticket.");
            }

            // if (ticket.Status == TicketStatus.Cancelled)
            // {
            //     throw new BusinessRuleException("Comments can not be added to a cancelled ticket.");
            // }

            var comment = new TicketComment
            {
                TicketId = ticket.Id,
                AuthorUserId = UserId,
                Content = request.Request.Content,
                Visibility = CommentVisibility.Public
            };

            dbContext.TicketComments.Add(comment);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new AddCustomerCommentResult
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