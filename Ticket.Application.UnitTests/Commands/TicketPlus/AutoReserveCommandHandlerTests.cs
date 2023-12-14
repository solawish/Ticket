using AutoFixture;
using AutoFixture.Xunit2;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
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
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithNoAreaName_WithNoCheckCount_WithNoAnyCache_ReturnReserveSuccess(
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

    [Theory]
    [AutoTestingData]
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithAreaName_WithNoCheckCount_WithNoAnyCache_HaveCorrespondAreaName_ShouldUseCorrectArea_ReturnReserveSuccess(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        AutoReserveCommandHandler sut
        )
    {
        // Arrange
        var expectedAreaName = "VIPAreaName";
        var request = fixture
            .Build<AutoReserveCommand>()
            .With(x => x.AreaName, expectedAreaName)
            .With(x => x.IsCheckCount, false)
            .Create();

        var product = fixture
            .Build<Product>()
            .CreateMany(3);
        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .With(x => x.Products, product)
            .Create();
        var getProductConfigDto = fixture
            .Build<GetProductConfigDto>()
            .Create();
        var tiackeArea = fixture
            .Build<TicketArea>()
            .WithValues(x => x.TicketAreaName, fixture.Create<string>(), expectedAreaName, fixture.Create<string>())
            .WithValues(x => x.Id, product.Select(x => x.TicketAreaId).ToArray())
            .CreateMany(3);
        var getAreaConfigDto = fixture
            .Build<GetAreaConfigDto>()
            .With(x => x.Result, new AreaConfiResult { TicketArea = tiackeArea.ToList() })
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

        // 確認送出預定的資料的productId有符合正確的ticketAreaName對應的ProductId
        mediator.Verify(x => x.Send(It.Is<CreateReserveCommand>(x =>
            x.Products.First().ProductId.Equals(
                product.First(x => x.TicketAreaId.Equals(
                    tiackeArea.First(x => x.TicketAreaName.Equals(expectedAreaName)).Id)).ProductId)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithAreaName_WithNoCheckCount_WithNoAnyCache_HaveSimilarAreaName_ShouldUseCorrectArea_ReturnReserveSuccess(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        AutoReserveCommandHandler sut
        )
    {
        // Arrange
        var specificAreaName = "VIPAreaName";
        var expectedAreaName = "VIP";
        var request = fixture
            .Build<AutoReserveCommand>()
            .With(x => x.AreaName, expectedAreaName)
            .With(x => x.IsCheckCount, false)
            .Create();

        var product = fixture
            .Build<Product>()
            .CreateMany(3);
        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .With(x => x.Products, product)
            .Create();
        var getProductConfigDto = fixture
            .Build<GetProductConfigDto>()
            .Create();
        var tiackeArea = fixture
            .Build<TicketArea>()
            .WithValues(x => x.TicketAreaName, fixture.Create<string>(), specificAreaName, fixture.Create<string>())
            .WithValues(x => x.Id, product.Select(x => x.TicketAreaId).ToArray())
            .CreateMany(3);
        var getAreaConfigDto = fixture
            .Build<GetAreaConfigDto>()
            .With(x => x.Result, new AreaConfiResult { TicketArea = tiackeArea.ToList() })
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

        // 確認送出預定的資料的productId有符合包含的ticketAreaName的對應的ProductId
        mediator.Verify(x => x.Send(It.Is<CreateReserveCommand>(x =>
            x.Products.First().ProductId.Equals(
                product.First(x => x.TicketAreaId.Equals(
                    tiackeArea.First(x => x.TicketAreaName.Contains(expectedAreaName)).Id)).ProductId)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithAreaName_WithNoCheckCount_WithNoAnyCache_NotHaveCorrespondAreaName_HaveCorrespondTicketName_ShouldUseCorrectTicketName_ReturnReserveSuccess(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        AutoReserveCommandHandler sut
        )
    {
        // Arrange
        var expectedTicketName = "VIPTicketName";
        var request = fixture
            .Build<AutoReserveCommand>()
            .With(x => x.AreaName, expectedTicketName)
            .With(x => x.IsCheckCount, false)
            .Create();

        var product = fixture
            .Build<Product>()
            .WithValues(x => x.Name, fixture.Create<string>(), expectedTicketName, fixture.Create<string>())
            .CreateMany(3);
        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .With(x => x.Products, product)
            .Create();
        var getProductConfigDto = fixture
            .Build<GetProductConfigDto>()
            .Create();
        var tiackeArea = fixture
            .Build<TicketArea>()
            //.WithValues(x => x.TicketAreaName, fixture.Create<string>(), expectedAreaName, fixture.Create<string>())
            //.WithValues(x => x.Id, product.Select(x => x.TicketAreaId).ToArray())
            .CreateMany(3);
        var getAreaConfigDto = fixture
            .Build<GetAreaConfigDto>()
            .With(x => x.Result, new AreaConfiResult { TicketArea = tiackeArea.ToList() })
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

        // 確認送出預定的資料的productId有符合的ticketName的對應的ProductId
        mediator.Verify(x => x.Send(It.Is<CreateReserveCommand>(x =>
            x.Products.First().ProductId.Equals(
                product.First(x => x.Name.Equals(expectedTicketName)).ProductId)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithAreaName_WithNoCheckCount_WithNoAnyCache_NotHaveCorrespondAreaName_HaveSimilarTicketName_ShouldUseCorrectTicketName_ReturnReserveSuccess(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        AutoReserveCommandHandler sut
        )
    {
        // Arrange
        var specificTicketName = "VIPTicketNamePlus";
        var expectedTicketName = "VIPTicketName";
        var request = fixture
            .Build<AutoReserveCommand>()
            .With(x => x.AreaName, expectedTicketName)
            .With(x => x.IsCheckCount, false)
            .Create();

        var product = fixture
            .Build<Product>()
            .WithValues(x => x.Name, fixture.Create<string>(), specificTicketName, fixture.Create<string>())
            .CreateMany(3);
        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .With(x => x.Products, product)
            .Create();
        var getProductConfigDto = fixture
            .Build<GetProductConfigDto>()
            .Create();
        var tiackeArea = fixture
            .Build<TicketArea>()
            //.WithValues(x => x.TicketAreaName, fixture.Create<string>(), expectedAreaName, fixture.Create<string>())
            //.WithValues(x => x.Id, product.Select(x => x.TicketAreaId).ToArray())
            .CreateMany(3);
        var getAreaConfigDto = fixture
            .Build<GetAreaConfigDto>()
            .With(x => x.Result, new AreaConfiResult { TicketArea = tiackeArea.ToList() })
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

        // 確認送出預定的資料的productId有符合包含的ticketName的對應的ProductId
        mediator.Verify(x => x.Send(It.Is<CreateReserveCommand>(x =>
            x.Products.First().ProductId.Equals(
                product.First(x => x.Name.Contains(expectedTicketName)).ProductId)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithAreaName_WithNoCheckCount_WithNoAnyCache_NotHaveCorrespondAreaName_NotHaveCorrespondTicketName_ShouleUseFirstProductId_ReturnReserveSuccess(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        AutoReserveCommandHandler sut
        )
    {
        // Arrange
        var specificAreaName = "VIPAreaName";
        var specificTicketName = "VIPTicketNamePlus";
        var requestAreaName = "NoEqualNameQAQ";
        var request = fixture
            .Build<AutoReserveCommand>()
            .With(x => x.AreaName, requestAreaName)
            .With(x => x.IsCheckCount, false)
            .Create();

        var product = fixture
            .Build<Product>()
            .WithValues(x => x.Name, fixture.Create<string>(), specificTicketName, fixture.Create<string>())
            .CreateMany(3);
        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .With(x => x.Products, product)
            .Create();
        var getProductConfigDto = fixture
            .Build<GetProductConfigDto>()
            .Create();
        var tiackeArea = fixture
            .Build<TicketArea>()
            .WithValues(x => x.TicketAreaName, fixture.Create<string>(), specificAreaName, fixture.Create<string>())
            .WithValues(x => x.Id, product.Select(x => x.TicketAreaId).ToArray())
            .CreateMany(3);
        var getAreaConfigDto = fixture
            .Build<GetAreaConfigDto>()
            .With(x => x.Result, new AreaConfiResult { TicketArea = tiackeArea.ToList() })
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

        // 確認送出預定的資料的productId有符合正確的ticketAreaName對應的ProductId
        mediator.Verify(x => x.Send(It.Is<CreateReserveCommand>(x =>
            x.Products.First().ProductId.Equals(product.First().ProductId)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithNoAreaName_WithCheckCount_WithNoAnyCache_DefaultProductHasCount_SholuedChooseFirstProduct_ReturnReserveSuccess(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IMemoryCache> memoryCache,
        AutoReserveCommandHandler sut
        )
    {
        // Arrange
        var request = fixture
            .Build<AutoReserveCommand>()
            .With(x => x.AreaName, "")
            .With(x => x.IsCheckCount, true)
            .Create();

        var products = fixture.Build<Product>()
            .CreateMany(3);
        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .With(x => x.Products, products)
            .Create();
        var productConfig = fixture
            .Build<ProductConfig>()
            .WithValues(x => x.Id, products.Select(x => x.ProductId).ToArray())
            .WithValues(x => x.Count, 100, 0, 100)
            .CreateMany(3);
        var getProductConfigDto = fixture
            .Build<GetProductConfigDto>()
            .With(x => x.Result, new Result { Product = productConfig.ToList() })
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

        memoryCache.Setup(x => x.TryGetValue(Ticket.Application.Common.Const.ProductConfigCacheKey, out getProductConfigDto))
            .Returns(true);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.CreateReserveDto.ErrCode.ShouldBe(Const.SuccessCode);
        result.CreateReserveDto.Products.ShouldNotBeNull();
        result.CreateReserveDto.Products.Count.ShouldBe(1);
        result.CreateReserveDto.Products.First().Status.ShouldBe(OrderStatusEnum.RESERVED.ToString());
        mediator.Verify(x => x.Send(It.Is<CreateReserveCommand>(x =>
                   x.Products.First().ProductId.Equals(products.First().ProductId)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handle_AutoReserveCommandHandler_GiveValidRequest_WithNoAreaName_WithCheckCount_WithNoAnyCache_DefaultProductHasNoCount_AnotherProductHasCount_SholuedChooseAnotherProduct_ReturnReserveSuccess(
        IFixture fixture,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IMemoryCache> memoryCache,
        AutoReserveCommandHandler sut
        )
    {
        // Arrange
        var request = fixture
            .Build<AutoReserveCommand>()
            .With(x => x.AreaName, "")
            .With(x => x.IsCheckCount, true)
            .Create();

        var products = fixture.Build<Product>()
            .CreateMany(3);
        var getS3ProductInfoDto = fixture
            .Build<GetS3ProductInfoDto>()
            .With(x => x.Products, products)
            .Create();
        var productConfig = fixture
            .Build<ProductConfig>()
            .WithValues(x => x.Id, products.Select(x => x.ProductId).ToArray())
            .WithValues(x => x.Count, 0, 0, 100)
            .CreateMany(3);
        var getProductConfigDto = fixture
            .Build<GetProductConfigDto>()
            .With(x => x.Result, new Result { Product = productConfig.ToList() })
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

        memoryCache.Setup(x => x.TryGetValue<GetProductConfigDto>(Ticket.Application.Common.Const.ProductConfigCacheKey, out getProductConfigDto))
            .Returns(true);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.CreateReserveDto.ErrCode.ShouldBe(Const.SuccessCode);
        result.CreateReserveDto.Products.ShouldNotBeNull();
        result.CreateReserveDto.Products.Count.ShouldBe(1);
        result.CreateReserveDto.Products.First().Status.ShouldBe(OrderStatusEnum.RESERVED.ToString());
        mediator.Verify(x => x.Send(It.Is<CreateReserveCommand>(x =>
                   x.Products.First().ProductId.Equals(productConfig.First(x => x.Count > 0).Id)), It.IsAny<CancellationToken>()), Times.Once);
    }
}