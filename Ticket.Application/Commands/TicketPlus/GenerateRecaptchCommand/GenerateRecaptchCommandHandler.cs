using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Commands.TicketPlus.GenerateRecaptchCommand;

/// <summary>
/// 產生驗證碼圖片
/// </summary>
public class GenerateRecaptchCommandHandler : IRequestHandler<GenerateRecaptchCommand, GenerateRecaptchDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;

    public GenerateRecaptchCommandHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions)
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
    }

    public async Task<GenerateRecaptchDto> Handle(GenerateRecaptchCommand request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_ticketPlusOptions.Value.GenerateRecaptchaUrl}")
        {
            Content = JsonContent.Create(
                request,
                options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }),
            Headers =
            {
                { "Authorization", $"Bearer {request.Token}" }
            }
        };

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GenerateRecaptchDto>(cancellationToken: cancellationToken);
    }
}