using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Shouldly;
using System.Text.Json;
using Ticket.Application.Commands.TicketPlus.GenerateCaptcha;
using Ticket.Application.Options;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Xunit;

namespace Ticket.Application.UnitTests.Commands.TicketPlus;

public class GenerateCaptchaHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_GenerateCaptcha_GiveValidRequest_ShouldReturnCaptcha(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        IOptions<TicketPlusOptions> options,
        GenerateCaptchaCommandHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<GenerateCaptchaCommand>();
        var response = fixture
            .Build<GenerateCaptchaDto>()
            .Create();

        var mockHttp = new MockHttpMessageHandler();
        var mockRequest = mockHttp
            .When(HttpMethod.Post, options.Value.GenerateCaptchaUrl)
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