using MediatR;

namespace SupportTicketing.Application.Features.Customer.Queries.GetAvailableCategories;

public record GetAvailableCategoriesQuery
    : IRequest<GetAvailableCategoriesResult>;