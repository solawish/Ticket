using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Ticket.Application.Commands.TicketPlus.CreateReserve;
using Ticket.Application.Options;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Xunit;

namespace Ticket.Application.UnitTests.Commands.TicketPlus;

[ExcludeFromCodeCoverage]
public class CreateReserveHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_CreateReserve_GiveValidRequest_ShouldReturnReserve(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        IOptions<TicketPlusOptions> options,
        CreateReserveCommandHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<CreateReserveCommand>();
        var response = fixture
            .Build<CreateReserveDto>()
            .Create();

        var mockHttp = new MockHttpMessageHandler();
        var mockRequest = mockHttp
            .When(HttpMethod.Post, options.Value.ReserveUrl)
            .WithHeaders("Authorization", $"Bearer {request.Token}")
            .WithJsonContent(request, new JsonSerializerOptions(JsonSerializerDefaults.Web))
            .Respond("application/json", JsonSerializer.Serialize(response));

        var httpClient = new HttpClient(mockHttp);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        mockHttp.GetMatchCount(mockRequest).ShouldBe(1);
    }
}