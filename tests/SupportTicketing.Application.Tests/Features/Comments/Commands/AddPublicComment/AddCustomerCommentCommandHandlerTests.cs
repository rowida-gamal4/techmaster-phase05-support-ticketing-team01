using Microsoft.EntityFrameworkCore;
using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Customer;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Comments.Commands.AddCustomerComment;
using SupportTicketing.Application.Tests.Common;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Comments.Commands.AddPublicComment
{
    public class AddCustomerCommentCommandHandlerTests
    {
        [Fact]
        public async Task ClosedTicketCannotReceiveComment()
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
                Title = "Closed Ticket test",
                Description = "Test ticket",
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

            var validator = new AddCustomerCommentValidator();

            var request = new AddCommentRequestDto
            {
                Content = "Adding comment to closed ticket test."
            };

            var command = new AddCustomerCommentCommand(ticket.Id, request);

            var handler = new AddCustomerCommentCommandHandler(currentUserService.Object, dbContext, validator);

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => handler.Handle(command, CancellationToken.None));

            Assert.Equal("Comments can not be added to a closed ticket.", exception.Message);

            Assert.Empty(await dbContext.TicketComments.ToListAsync());

        }
    }
}
