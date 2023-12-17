using AutoFixture;
using AutoFixture.Xunit2;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Ticket.Application.Commands.TicketPlus.InitialUser;
using Ticket.Application.Common.TicketPlus;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Xunit;

namespace Ticket.Application.UnitTests.Commands.TicketPlus;

public class InitialUserCacheCommandTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_InitialUserCache_GiveValidRequest_ShouldAddInitialUserCache(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IMemoryCache> memoryCache,
        InitialUserCacheCommandHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<InitialUserCacheCommand>();
        var response = fixture
            .Build<InitialUserCacheDto>()
            .Create();

        var accessTokenDto = fixture
            .Build<GetAccessTokenDto>()
            .Create();

        mediator.Setup(x => x.Send(It.IsAny<GetAccessTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessTokenDto);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<GetAccessTokenQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        memoryCache.Verify(x => x.CreateEntry(string.Format(CacheKey.UserCacheKey, request.Mobile)), Times.Once);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handle_InitialUserCache_GiveInvalidRequest_ShouldThrowArgumentException(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IMemoryCache> memoryCache,
        InitialUserCacheCommandHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<InitialUserCacheCommand>();
        var response = fixture
            .Build<InitialUserCacheDto>()
            .Create();

        var accessTokenDto = fixture
            .Build<GetAccessTokenDto>()
            .Without(x => x.UserInfo)
            .Create();

        mediator.Setup(x => x.Send(It.IsAny<GetAccessTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessTokenDto);

        // Act
        var result = await Assert.ThrowsAsync<ArgumentException>(() => sut.Handle(request, CancellationToken.None));

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<GetAccessTokenQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        memoryCache.Verify(x => x.CreateEntry(string.Format(CacheKey.UserCacheKey, request.Mobile)), Times.Never);
    }
}