using AutoFixture;
using AutoFixture.Xunit2;
using Moq;
using RichardSzalay.MockHttp;
using Shouldly;
using System.Text.Json;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Xunit;

namespace Ticket.Application.UnitTests.Queries.TicketPlus;

public class GetAccessTokenHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_GetAccessTokenHandler_GiveValidRequest_ShouldReturnAccessToken(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        GetAccessTokenHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<GetAccessTokenQuery>();
        var response = fixture
            .Build<GetAccessTokenDto>()
            .With(x => x.ErrCode, "00")
            .Create();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://apis.ticketplus.com.tw/user/api/v1/login")
            .Respond("application/json", JsonSerializer.Serialize(response)); // Respond with JSON

        var httpClient = new HttpClient(mockHttp);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ErrCode.ShouldBe("00");
    }
}