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
using Xunit;

namespace Ticket.Application.UnitTests.Queries.TicketPlus;

[ExcludeFromCodeCoverage]
public class GetS3ProductInfoQueryHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handler_GetS3ProductInfoQueryHandler_GiveValidRequest_ShouldReturnS3ProductInfo(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        IOptions<TicketPlusOptions> options,
        GetS3ProductInfoQueryHandler sut)
    {
        // Arrange
        var request = fixture.Create<GetS3ProductInfoQuery>();
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
}