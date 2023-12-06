using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Queries.TicketPlus.GetProductInfoQuery;

/// <summary>
/// 取得活動資訊
/// </summary>
public class GetProductInfoQueryHandler : IRequestHandler<GetProductInfoQuery, GetProductInfoDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;

    public GetProductInfoQueryHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions)
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
    }

    public async Task<GetProductInfoDto> Handle(GetProductInfoQuery request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        // todo : this pathArgu should add some options (ex. products, sessions, event ...etc )
        var pahtArgu = $"event/{request.ActivityId}/products.json";
        var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_ticketPlusOptions.Value.ConfigUrl}?path={pahtArgu}");

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetProductInfoDto>(cancellationToken: cancellationToken);
        return result;
    }
}