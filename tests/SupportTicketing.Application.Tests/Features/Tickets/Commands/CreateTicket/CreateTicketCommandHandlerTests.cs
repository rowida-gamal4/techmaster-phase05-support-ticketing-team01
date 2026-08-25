using Microsoft.EntityFrameworkCore;
using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Features.Tickets.Commands.CreateTicket;
using SupportTicketing.Application.Tests.Common;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommandHandlerTests
    {
        [Fact]
        public async Task CeateTicketWithValidDataSucceeds()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            await using var dbContext = new TestDbContext(options);

            var customer = new CustomerProfile
            {
                Id = 1,
                UserId = 10
            };

            var category = new TicketCategory
            {
                Id = 1,
                Name = "Payments Support",
                Code = "p-2",
                IsActive = true
            };

            dbContext.CustomerProfiles.Add(customer);
            dbContext.TicketCategories.Add(category);
            await dbContext.SaveChangesAsync();

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(x => x.IsAuthenticated).Returns(true);

            currentUserService.Setup(x => x.UserId).Returns(10);

            var validator = new CreateTicketCommandValidator();

            var handler = new CreateTicketCommandHandler(currentUserService.Object, dbContext, validator);

            var request = new CreateTicketRequestDto
            {
                Title = "Payment Problem",
                Description = "I have a problem with my payment.",
                CategoryId = 1
            };

            var command = new CreateTicketCommand(request);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result.Ticket);

            var ticketInDatabase = await dbContext.Tickets.FirstOrDefaultAsync();

            Assert.NotNull(ticketInDatabase);
            Assert.Equal(TicketStatus.New, ticketInDatabase.Status);

        }
    }
}