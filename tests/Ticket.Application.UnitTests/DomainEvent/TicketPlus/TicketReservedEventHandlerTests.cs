using AutoFixture;
using Ticket.Application.DomainEventHandlers.TicketPlus;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Ticket.Domain.Events.TicketPlus;
using Xunit;

namespace Ticket.Application.UnitTests.DomainEvent.TicketPlus;

public class TicketReservedEventHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_WhenTicketReservedEvent_ShouldLogInformation(
        IFixture fixture,
        TicketReservedEventHandler sut
        )
    {
        // Arrange
        var ticketReserveEvent = fixture.Create<TicketReservedEvent>();

        // Act
        await sut.Handle(ticketReserveEvent, CancellationToken.None);

        // Assert
    }
}