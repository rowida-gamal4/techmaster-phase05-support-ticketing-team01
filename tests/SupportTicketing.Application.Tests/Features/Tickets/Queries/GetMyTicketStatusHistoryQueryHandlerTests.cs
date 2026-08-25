using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Tickets.Queries.GetMyTicketStatusHistory;
using SupportTicketing.Application.Tests.Common;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Tickets.Queries;

public class GetMyTicketStatusHistoryQueryHandlerTests
{
    [Fact]
    public async Task Customer_CannotAccessAnotherCustomersTicket()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        await using var dbContext = new TestDbContext(options);

        var firstCustomer = new CustomerProfile
        {
            Id = 1,
            UserId = 10
        };

        var secondCustomer = new CustomerProfile
        {
            Id = 2,
            UserId = 20
        };

        var ticket = new Ticket
        {
            Id = 1,
            CustomerId = secondCustomer.Id,
            Title = "Customer 2 Ticket",
            Description = "Private ticket",
            CategoryId = 1,
            Status = TicketStatus.New,
            Priority = TicketPriority.Low
        };

        ticket.SetCreatedAt();

        dbContext.CustomerProfiles.AddRange(firstCustomer, secondCustomer);

        dbContext.Tickets.Add(ticket);

        await dbContext.SaveChangesAsync();

        var currentUserService = new Mock<ICurrentUserService>();

        currentUserService.Setup(x => x.IsAuthenticated).Returns(true);

        currentUserService.Setup(x => x.UserId).Returns(10);

        currentUserService.Setup(x => x.Role).Returns(Roles.Customer);

        var validator = new GetMyTicketStatusHistoryQueryValidator();

        var handler = new GetMyTicketStatusHistoryQueryHandler(currentUserService.Object, dbContext, validator);


        var query = new GetMyTicketStatusHistoryQuery(ticket.Id);



        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}