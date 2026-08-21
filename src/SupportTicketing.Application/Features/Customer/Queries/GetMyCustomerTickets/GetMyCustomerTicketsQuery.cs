using MediatR;
using SupportTicketing.Application.DTOs.Tickets;

namespace SupportTicketing.Application.Features.Tickets.Queries.GetMyCustomerTickets;

public record GetMyCustomerTicketsQuery(GetMyTicketsRequestDto Request) : IRequest<GetMyCustomerTicketsResult>;