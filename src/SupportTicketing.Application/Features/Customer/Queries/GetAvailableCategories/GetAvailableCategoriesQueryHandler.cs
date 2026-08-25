using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.TicketCategories;

namespace SupportTicketing.Application.Features.Customer.Queries.GetAvailableCategories;

public class GetAvailableCategoriesQueryHandler
    : IRequestHandler<GetAvailableCategoriesQuery, GetAvailableCategoriesResult>
{
    private readonly IApplicationDbContext dbContext;
    public GetAvailableCategoriesQueryHandler(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<GetAvailableCategoriesResult> Handle(GetAvailableCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await dbContext.TicketCategories.AsNoTracking()
            .Where(c => c.IsActive).OrderBy(c => c.Name).Select(c => new AvailableCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description
            }).ToListAsync(cancellationToken);

        return new GetAvailableCategoriesResult
        {
            Categories = categories
        };
    }
}