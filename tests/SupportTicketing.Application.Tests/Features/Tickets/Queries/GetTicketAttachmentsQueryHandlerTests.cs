using Microsoft.EntityFrameworkCore;
using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Features.Tickets.Queries.GetTicketAttachments;
using SupportTicketing.Application.Tests.Common;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Tickets.Queries
{
    public class GetTicketAttachmentsQueryHandlerTests
    {
        [Fact]
        public async Task AgentCannotAccessUnassignedTicketAttachments()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            await using var dbContext = new TestDbContext(options);

            var agent = new AgentProfile
            {
                Id = 1,
                UserId = 10,
                FullName = "Zeina",
                IsActive = true
            };
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
                Title = "Customer Ticket",
                Description = "Test ticket",
                Status = TicketStatus.New,
                Priority = TicketPriority.High
            };
            ticket.SetCreatedAt();

            var attachment = new TicketAttachmentMetadata
            {
                Id = 1,
                TicketId = ticket.Id,
                UploadedByUserId = customer.UserId,
                FileName = "test.pdf",
                FileSize = 1000,
                ContentType = "application/pdf",
                StorageKey = $"tickets/{ticket.Id}/test.pdf"
            };

            dbContext.AgentProfiles.Add(agent);
            dbContext.CustomerProfiles.Add(customer);
            dbContext.Tickets.Add(ticket);
            dbContext.TicketAttachments.Add(attachment);

            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(x => x.IsAuthenticated).Returns(true);
            currentUserService.Setup(x => x.UserId).Returns(agent.UserId);
            currentUserService.Setup(x => x.Role).Returns(Roles.SupportAgent);

            var validator = new GetTicketAttachmentsValidator();

            var request = new GetAttachmentRequestDto
            {
                TicketId = ticket.Id
            };
            var query = new GetTicketAttachmentsQuery(request);
            var handler = new GetTicketAttachmentsQueryHandler(currentUserService.Object, dbContext, validator);
            var result = await handler.Handle(query, CancellationToken.None);
            Assert.Empty(result.Attachments);
        }
    }

}