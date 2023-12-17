using AutoFixture;
using AutoFixture.Xunit2;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Ticket.Application.Commands.TicketPlus.InitialActivityCache;
using Ticket.Application.Common.TicketPlus;
using Ticket.Application.Queries.TicketPlus.GetAreaConfig;
using Ticket.Application.Queries.TicketPlus.GetProductConfig;
using Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Xunit;

namespace Ticket.Application.UnitTests.Commands.TicketPlus;

public class InitialActivityCacheCommandHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_InitialActivityCache_GiveValidRequest_ShouldAddInitialActivityCache(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IMemoryCache> memoryCache,
        InitialActivityCacheCommandHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<InitialActivityCacheCommand>();
        var response = fixture
            .Build<InitialActivityCacheDto>()
            .Create();

        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .Create();
        var ticketConfigQueryDto = fixture
            .Build<GetProductConfigDto>()
            .Create();
        var areaConfigQueryDto = fixture
            .Build<GetAreaConfigDto>()
            .Create();

        mediator.Setup(x => x.Send(It.IsAny<GetS3ProductInfoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getS3ProductInfoDto);
        mediator.Setup(x => x.Send(It.IsAny<GetProductConfigQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketConfigQueryDto);
        mediator.Setup(x => x.Send(It.IsAny<GetAreaConfigQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(areaConfigQueryDto);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<GetS3ProductInfoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        mediator.Verify(x => x.Send(It.IsAny<GetProductConfigQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        mediator.Verify(x => x.Send(It.IsAny<GetAreaConfigQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        memoryCache.Verify(x => x.CreateEntry(string.Format(CacheKey.S3ProductInfoCacheKey, request.ActivityId)), Times.Once);
        memoryCache.Verify(x => x.CreateEntry(string.Format(CacheKey.ProductConfigCacheKey, request.ActivityId)), Times.Once);
        memoryCache.Verify(x => x.CreateEntry(string.Format(CacheKey.AreaConfigCacheKey, request.ActivityId)), Times.Once);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handle_InitialActivityCache_GiveInvalidRequest_ShouldThrowArgumentException(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IMemoryCache> memoryCache,
        InitialActivityCacheCommandHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<InitialActivityCacheCommand>();
        var response = fixture
            .Build<InitialActivityCacheDto>()
            .Create();

        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .Without(x => x.Products)
            .Create();

        mediator.Setup(x => x.Send(It.IsAny<GetS3ProductInfoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getS3ProductInfoDto);

        // Act
        var result = await Assert.ThrowsAsync<ArgumentException>(() => sut.Handle(request, CancellationToken.None));

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<GetS3ProductInfoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        memoryCache.Verify(x => x.CreateEntry(string.Format(CacheKey.S3ProductInfoCacheKey, request.ActivityId)), Times.Never);
    }
}