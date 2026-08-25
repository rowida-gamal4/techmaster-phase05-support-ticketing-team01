using Microsoft.EntityFrameworkCore;
using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.DTOs.TicketComment;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Comments.Commands.AddInternalNote;
using SupportTicketing.Application.Tests.Common;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Comments.Commands.AddInternalNote
{
    public class AddInternalNoteCommandHandlerTests
    {
        [Fact]
        public async Task CustomerCannotAddInternalNote()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            await using var dbContext = new TestDbContext(options);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(x => x.IsAuthenticated).Returns(true);
            currentUserService.Setup(x => x.UserId).Returns(20);
            currentUserService.Setup(x => x.Role).Returns(Roles.Customer);

            var validator = new AddInternalNoteCommandValidator();
            var handler = new AddInternalNoteCommandHandler(currentUserService.Object, dbContext, validator);

            var request = new AddInternalNoteRequestDto
            {
                Content = "Testing  Customer Can not add Internal note."
            };

            var command = new AddInternalNoteCommand(1, request);

            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));

        }
    }
}