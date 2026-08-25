using Microsoft.EntityFrameworkCore;
using SupportTicketing.Application.Tests.Common;
using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.TicketAssignment;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Tickets.Commands.AssignTicket;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Tickets.AssignTicket.Commands
{
    public class AssignTicketCommandHandlerTests
    {
        [Fact]
        public async Task CannotAssignInactiveAgent()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            await using var dbContext = new TestDbContext(options);

            var agent = new AgentProfile
            {
                Id = 1,
                UserId = 10,
                FullName = "Inactive Agent",
                IsActive = false
            };

            var customer = new CustomerProfile
            {
                Id = 1,
                UserId = 10
            };
            var team = new SupportTeam
            {
                Id = 1,
                Name = "Support Team",
                IsActive = true
            };

            var ticket = new Ticket
            {
                Id = 1,
                CustomerId = customer.Id,
                CategoryId = 1,
                Title = "Payment Problem",
                Description = "Customer has a payment problem.",
                Status = TicketStatus.New,
                Priority = TicketPriority.Low
            };


            ticket.SetCreatedAt();

            dbContext.AgentProfiles.Add(agent);
            dbContext.CustomerProfiles.Add(customer);
            dbContext.SupportTeams.Add(team);
            dbContext.Tickets.Add(ticket);

            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(x => x.IsAuthenticated).Returns(true);
            currentUserService.Setup(x => x.UserId).Returns(100);
            currentUserService.Setup(x => x.Role).Returns(Roles.Admin);

            var validator = new AssignTicketCommandValidator();

            var command = new AssignTicketCommand(ticket.Id, new AssignTicketRequestDto
            {
                AgentId = agent.Id,
                TeamId = team.Id
            });

            var handler = new AssignTicketCommandHandler(currentUserService.Object, dbContext, validator);
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => handler.Handle(command, CancellationToken.None));

            Assert.Equal("Agent not active", exception.Message);

            var assignment = await dbContext.TicketAssignments.FirstOrDefaultAsync();
            Assert.Null(assignment);

        }
    }
}