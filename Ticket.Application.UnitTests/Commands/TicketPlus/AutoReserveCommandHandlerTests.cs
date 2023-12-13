using AutoFixture;
using AutoFixture.Xunit2;
using MediatR;
using Moq;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using Ticket.Application.Commands.TicketPlus.AutoReserve;
using Ticket.Application.Commands.TicketPlus.CreateReserve;
using Ticket.Application.Commands.TicketPlus.GenerateCaptcha;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;
using Ticket.Application.Queries.TicketPlus.GetAreaConfig;
using Ticket.Application.Queries.TicketPlus.GetCaptchaAnswer;
using Ticket.Application.Queries.TicketPlus.GetProductConfig;
using Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Ticket.Domain.Common.TicketPlus;
using Ticket.Domain.Entities.TicketPlus;
using Ticket.Domain.Enum.TicketPlus;
using Xunit;

namespace Ticket.Application.UnitTests.Commands.TicketPlus;

[ExcludeFromCodeCoverage]
public class AutoReserveCommandHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithNoAreaName_WItnNoCheckCount_WithNoAnyCache_ReturnReserveSuccess(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        AutoReserveCommandHandler sut
        )
    {
        // Arrange
        var request = fixture
            .Build<AutoReserveCommand>()
            .With(x => x.AreaName, "")
            .With(x => x.IsCheckCount, false)
            .Create();

        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .Create();
        var getProductConfigDto = fixture
            .Build<GetProductConfigDto>()
            .Create();
        var getAreaConfigDto = fixture
            .Build<GetAreaConfigDto>()
            .Create();
        var accessTokenDto = fixture
            .Build<GetAccessTokenDto>()
            .Create();
        var generateCaptchaDto = fixture
            .Build<GenerateCaptchaDto>()
            .Create();
        var getCaptchaAnswerDto = fixture
            .Build<GetCaptchaAnswerDto>()
            .Create();
        var reserveProductDto = fixture
            .Build<OrderProduct>()
            .With(x => x.Status, OrderStatusEnum.RESERVED.ToString())
            .Create();
        var createReserveDto = fixture
            .Build<CreateReserveDto>()
            .With(x => x.ErrCode, Const.SuccessCode)
            .With(x => x.Products, new List<OrderProduct> { reserveProductDto })
            .Create();

        mediator.Setup(x => x.Send(It.IsAny<GetS3ProductInfoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getS3ProductInfoDto);
        mediator.Setup(x => x.Send(It.IsAny<GetProductConfigQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getProductConfigDto);
        mediator.Setup(x => x.Send(It.IsAny<GetAreaConfigQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getAreaConfigDto);
        mediator.Setup(x => x.Send(It.IsAny<GetAccessTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessTokenDto);
        mediator.Setup(x => x.Send(It.IsAny<GenerateCaptchaCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(generateCaptchaDto);
        mediator.Setup(x => x.Send(It.IsAny<GetCaptchaAnswerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getCaptchaAnswerDto);
        mediator.Setup(x => x.Send(It.IsAny<CreateReserveCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createReserveDto);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.CreateReserveDto.ErrCode.ShouldBe(Const.SuccessCode);
        result.CreateReserveDto.Products.ShouldNotBeNull();
        result.CreateReserveDto.Products.Count.ShouldBe(1);
        result.CreateReserveDto.Products.First().Status.ShouldBe(OrderStatusEnum.RESERVED.ToString());
    }
}