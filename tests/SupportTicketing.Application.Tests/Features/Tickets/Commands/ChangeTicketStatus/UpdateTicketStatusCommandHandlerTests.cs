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
    public class UpdateTicketStatusCommandHandlerTests
    {
        [Fact]
        public async Task NewTicketCannotBeClosedDirectly()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            await using var dbContext = new TestDbContext(options);

            var customer = new CustomerProfile
            {
                Id = 1,
                UserId = 10
            };
            var ticket = new Ticket
            {
                Id = 1,
                CustomerId = customer.Id,
                CategoryId = 1,
                Title = "Ticket Status Test",
                Description = "Test ticket",
                Status = TicketStatus.New,
                Priority = TicketPriority.Low
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
            var command = new UpdateTicketStatusCommand( ticket.Id,new UpdateTicketStatusRequestDto
                {
                    Status = "Closed"
                });
            var handler = new UpdateTicketStatusCommandHandler( currentUserService.Object, dbContext,validator);   

            await Assert.ThrowsAsync<BusinessRuleException>( () => handler.Handle(command, CancellationToken.None)); 
        }
    }
}