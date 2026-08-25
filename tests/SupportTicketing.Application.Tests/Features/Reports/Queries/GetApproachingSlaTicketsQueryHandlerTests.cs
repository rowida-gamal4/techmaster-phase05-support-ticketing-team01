using Moq;
using SupportTicketing.Application.Common.Interfaces;
using SupportTicketing.Application.Exceptions;
using SupportTicketing.Application.Features.Sla.Queries.GetSlaRiskReport;
using SupportTicketing.Application.Features.Tickets.Queries.GetSlaRiskReport;
using SupportTicketing.Domain.Enums;

namespace SupportTicketing.Application.Tests.Features.Reports.Queries
{
    public class GetApproachingSlaTicketsQueryHandlerTests
    {
        [Fact]
        public async Task CustomerCannotGetAdminSlaReport()
        {
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(x => x.IsAuthenticated).Returns(true);
            currentUserService.Setup(x => x.UserId).Returns(10);
            currentUserService.Setup(x => x.Role).Returns(Roles.Customer);

            var handler = new GetApproachingSlaTicketsQueryHandler(currentUserService.Object, null!);

            var query = new GetApproachingSlaTicketsQuery();

            var exception = await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(query, CancellationToken.None));

            Assert.Equal("Only Admin or SupportLead can view SLA risk tickets.", exception.Message);
        }
    }
}