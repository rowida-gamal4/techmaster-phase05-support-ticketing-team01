using Microsoft.EntityFrameworkCore;
using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Tickets.Commands.ChangeTicketStatus;
using SupportTicketing.Application.Tests.Common;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Tickets.Commands.ChangeTicketStatus
{
    public class ReopenTicketCommandHandlerTests
    {
        [Fact]
        public async Task CustomerCannotReopenClosedTicket()
        {

            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            await using var dbContext = new TestDbContext(options);

            var customer = new CustomerProfile
            {
                Id = 1,
                UserId = 20
            };

            var ticket = new Ticket
            {
                Id = 1,
                CustomerId = customer.Id,
                CategoryId = 1,
                Title = "Payment Problem",
                Description = "Payment issue",
                Status = TicketStatus.Closed,
                Priority = TicketPriority.High
            };

            ticket.SetCreatedAt();

            dbContext.CustomerProfiles.Add(customer);
            dbContext.Tickets.Add(ticket);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(x => x.IsAuthenticated).Returns(true);
            currentUserService.Setup(x => x.UserId).Returns(customer.UserId);
            currentUserService.Setup(x => x.Role).Returns(Roles.Customer);

            var validator = new UpdateTicketStatusValidator();

            var request = new UpdateTicketStatusRequestDto
            {
                Status = TicketStatus.Reopened.ToString()
            };

            var command = new UpdateTicketStatusCommand(ticket.Id, request);
            var handler = new UpdateTicketStatusCommandHandler(currentUserService.Object, dbContext, validator);


            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));
            var ticketInDatabase = await dbContext.Tickets.FirstAsync(t => t.Id == ticket.Id);
            Assert.Equal(TicketStatus.Closed, ticketInDatabase.Status);
        }
    }
}