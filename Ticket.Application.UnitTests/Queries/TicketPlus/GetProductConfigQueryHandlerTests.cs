using AutoFixture;
using AutoFixture.Xunit2;
using Moq;
using RichardSzalay.MockHttp;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Ticket.Application.Queries.TicketPlus.GetProductConfig;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Xunit;

namespace Ticket.Application.UnitTests.Queries.TicketPlus;

[ExcludeFromCodeCoverage]
public class GetProductConfigQueryHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_GetProductConfigQueryHandler_GiveValidRequest_ShouldReturnProductConfig(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        GetProductConfigQueryHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<GetProductConfigQuery>();
        var response = fixture
            .Build<GetProductConfigDto>()
            .With(x => x.ErrCode, "00")
            .Create();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://apis.ticketplus.com.tw/config/api/v1/get")
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