using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Ticket.Application.Options;
using Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Ticket.Domain.Entities.TicketPlus;
using Xunit;

namespace Ticket.Application.UnitTests.Queries.TicketPlus;

[ExcludeFromCodeCoverage]
public class GetS3ProductInfoQueryHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handler_GetS3ProductInfoQueryHandler_GiveValidRequest_WithoutSessionId_ShouldReturnS3ProductInfo(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        IOptions<TicketPlusOptions> options,
        GetS3ProductInfoQueryHandler sut)
    {
        // Arrange
        var request = fixture.Build<GetS3ProductInfoQuery>()
            .Without(x => x.SessionId)
            .Create();
        var response = fixture
            .Build<GetS3ProductInfoDto>()
            .Create();

        var mockHttp = new MockHttpMessageHandler();
        var mockRequest = mockHttp
            .When(HttpMethod.Get, options.Value.S3ConfigUrl)
            .WithQueryString("path", $"event/{request.ActivityId}/products.json")
            .Respond("application/json", JsonSerializer.Serialize(response));

        var httpClient = new HttpClient(mockHttp);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Products.ShouldNotBeNull();
        mockHttp.GetMatchCount(mockRequest).ShouldBe(1);
    }

    [Theory]
    [AutoTestingData]
    public async Task Handler_GetS3ProductInfoQueryHandler_GiveValidRequest_WithSessionId_ShouldReturnS3ProductInfoWithSpecificSessionId(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        IOptions<TicketPlusOptions> options,
        GetS3ProductInfoQueryHandler sut)
    {
        // Arrange
        var request = fixture.Build<GetS3ProductInfoQuery>()
            .With(x => x.SessionId)
            .Create();
        var responseProducts = fixture
            .Build<Product>()
            .WithValues(x => x.SessionId, request.SessionId, fixture.Create<string>())
            .CreateMany(2);
        var response = fixture
            .Build<GetS3ProductInfoDto>()
            .With(x => x.Products, responseProducts)
            .Create();

        var mockHttp = new MockHttpMessageHandler();
        var mockRequest = mockHttp
            .When(HttpMethod.Get, options.Value.S3ConfigUrl)
            .WithQueryString("path", $"event/{request.ActivityId}/products.json")
            .Respond("application/json", JsonSerializer.Serialize(response));

        var httpClient = new HttpClient(mockHttp);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Products.ShouldNotBeNull();
        result.Products.ShouldAllBe(x => x.SessionId.Equals(request.SessionId));
        mockHttp.GetMatchCount(mockRequest).ShouldBe(1);
    }
}