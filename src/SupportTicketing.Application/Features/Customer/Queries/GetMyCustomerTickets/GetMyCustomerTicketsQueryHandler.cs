using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Customer;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyCustomerTickets
{
    public class GetMyCustomerTicketsQueryHandler
    : IRequestHandler<GetMyCustomerTicketsQuery, GetMyCustomerTicketsResult>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;
        private readonly IValidator<GetMyCustomerTicketsQuery> validator;

        public GetMyCustomerTicketsQueryHandler(ICurrentUserService currentUserService , IApplicationDbContext dbContext,IValidator<GetMyCustomerTicketsQuery> validator)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
            this.validator = validator;
        }
        public async Task<GetMyCustomerTicketsResult> Handle(GetMyCustomerTicketsQuery request, CancellationToken cancellationToken)
        {
            var validationResult =  await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("Authentication is required.");
            }

            var UserId = currentUserService.UserId.Value ;

            var customer = await dbContext.CustomerProfiles.FirstOrDefaultAsync(c=>c.UserId == UserId , cancellationToken);

            if (customer is null)
            {
               throw new ForbiddenException("Customer profile was not found.");
            }

            var query = dbContext.Tickets.AsNoTracking().Where(t => t.CustomerId == customer.Id);

            if(!string.IsNullOrWhiteSpace(request.Request.Status))
            {
                if (!Enum.TryParse<TicketStatus>(request.Request.Status,true,out var status))
                {
                    throw new ArgumentException( "Invalid ticket status.");
                }
                query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Request.Priority))
            {
                if (!Enum.TryParse<TicketPriority>(request.Request.Priority,true,out var priority))
                {
                    throw new ArgumentException("Invalid ticket priority.");
                }

                query = query.Where(t => t.Priority == priority);
            }
            if (request.Request.CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == request.Request.CategoryId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var pageNumber = request.Request.PageNumber;
            var pageSize = request.Request.PageSize;

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var tickets = await query.OrderByDescending(t=>t.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(t=>new CustomerTicketResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CategoryId = t.CategoryId,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                CreatedAt = t.CreatedAt,
                StartedAt = t.StartedAt,
                ResolvedAt = t.ResolvedAt,
                ClosedAt = t.ClosedAt,
                CancelledAt = t.CancelledAt
            }).ToListAsync(cancellationToken);

            return new GetMyCustomerTicketsResult
            {      
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Tickets = tickets
            };
        }
    }
}