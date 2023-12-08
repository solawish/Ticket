using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Commands.TicketPlus.GenerateCaptcha;

/// <summary>
/// 產生驗證碼圖片
/// </summary>
public class GenerateCaptchaCommandHandler : IRequestHandler<GenerateCaptchaCommand, GenerateCaptchaDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;

    public GenerateCaptchaCommandHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions)
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
    }

    public async Task<GenerateCaptchaDto> Handle(GenerateCaptchaCommand request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Token}");

        var response = await httpClient.PostAsJsonAsync(
          _ticketPlusOptions.Value.GenerateCaptchaUrl,
          request,
          cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GenerateCaptchaDto>(cancellationToken: cancellationToken);
    }
}