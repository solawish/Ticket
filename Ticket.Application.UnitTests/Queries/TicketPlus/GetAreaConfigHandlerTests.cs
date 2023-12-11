using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Ticket.Application.Options;
using Ticket.Application.Queries.TicketPlus.GetAreaConfig;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Xunit;

namespace Ticket.Application.UnitTests.Queries.TicketPlus;

[ExcludeFromCodeCoverage]
public class GetAreaConfigHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_GetAreaConfigHandler_GiveValidRequest_ShouldReturnAreaConfig(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        IOptions<TicketPlusOptions> options,
        GetAreaConfigHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<GetAreaConfigQuery>();
        var response = fixture
            .Build<GetAreaConfigDto>()
            .Create();

        var mockHttp = new MockHttpMessageHandler();
        var mockRequest = mockHttp
            .When(HttpMethod.Get, options.Value.ConfigUrl)
            .WithQueryString("ticketAreaId", string.Join(',', request.TicketAreaId.Distinct()))
            .Respond("application/json", JsonSerializer.Serialize(response));

        var httpClient = new HttpClient(mockHttp);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Result.ShouldNotBeNull();
        result.Result.TicketArea.ShouldNotBeNull();
        mockHttp.GetMatchCount(mockRequest).ShouldBe(1);
    }
}