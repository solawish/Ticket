using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Ticket.Application.Helpers;
using Ticket.Application.Options;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;
using Ticket.Application.UnitTests.AutoFixtureSettings;
using Xunit;

namespace Ticket.Application.UnitTests.Queries.TicketPlus;

[ExcludeFromCodeCoverage]
public class GetAccessTokenHandlerTests
{
    [Theory]
    [AutoTestingData]
    public async Task Handle_GetAccessTokenHandler_GiveValidRequest_ShouldReturnAccessToken(
        IFixture fixture,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        [Frozen] Mock<IMD5Helper> md5Helper,
        IOptions<TicketPlusOptions> options,
        GetAccessTokenHandler sut
        )
    {
        // Arrange
        var request = fixture.Create<GetAccessTokenQuery>();
        var response = fixture
            .Build<GetAccessTokenDto>()
            .Create();

        var encryptedPassword = fixture.Create<string>();
        md5Helper.Setup(x => x.GetMd5Hash(It.IsAny<string>())).Returns(encryptedPassword);

        var mockHttp = new MockHttpMessageHandler();
        var mockRequest = mockHttp
            .When(HttpMethod.Post, options.Value.LoginUrl)
            .WithJsonContent<GetAccessTokenQuery>(x => x.Password.Equals(encryptedPassword), new JsonSerializerOptions(JsonSerializerDefaults.Web))
            .Respond("application/json", JsonSerializer.Serialize(response));

        var httpClient = new HttpClient(mockHttp);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.UserInfo.ShouldNotBeNull();
        mockHttp.GetMatchCount(mockRequest).ShouldBe(1);
    }
}