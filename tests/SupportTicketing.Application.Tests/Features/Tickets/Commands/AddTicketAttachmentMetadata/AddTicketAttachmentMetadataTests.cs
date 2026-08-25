using Microsoft.EntityFrameworkCore;
using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.Tickets;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Tickets.Commands.AddTicketAttachmentMetadata;
using SupportTicketing.Application.Tests.Common;
using SupportTicketing.Domain.Entities;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Tickets.Commands.AddTicketAttachmentMetadata
{
    public class AddTicketAttachmentMetadataTests
    {

        [Fact]
        public async Task CustomerCannotAddAttachmentToAnotherCustomerTicket()
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
                CategoryId = 1,
                Title = "Customer Two Ticket",
                Description = "Test Adding Attachment to Ticket.",
                Status = TicketStatus.New,
                Priority = TicketPriority.Low
            };

            ticket.SetCreatedAt();

            dbContext.CustomerProfiles.AddRange(firstCustomer, secondCustomer);
            dbContext.Tickets.Add(ticket);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(x => x.IsAuthenticated).Returns(true);
            currentUserService.Setup(x => x.UserId).Returns(firstCustomer.UserId);
            currentUserService.Setup(x => x.Role).Returns(Roles.Customer);

            var validator = new AddTicketAttachmentMetadataValidator();
            var request = new AddTicketAttachmentMetadataRequestDto
            {
                FileName = "test.pdf",
                FileSize = 1000,
                ContentType = "application/pdf"
            };

            var command = new AddTicketAttachmentMetadataCommand(ticket.Id, request);
            var handler = new AddTicketAttachmentMetadataCommandHandler(currentUserService.Object, dbContext, validator);

            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));

            var attachment = await dbContext.TicketAttachments.FirstOrDefaultAsync();
            Assert.Null(attachment);
        }
    }
}
