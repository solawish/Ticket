using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Queries.TicketPlus.GetAreaConfig;

/// <summary>
/// 取得票區的資訊
/// </summary>
public class GetAreaConfigHandler : IRequestHandler<GetAreaConfigQuery, GetAreaConfigDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;

    public GetAreaConfigHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions
        )
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
    }

    public async Task<GetAreaConfigDto> Handle(GetAreaConfigQuery request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        var response = await httpClient.GetAsync(
            $"{_ticketPlusOptions.Value.ConfigUrl}?ticketAreaId={string.Join(',', request.TicketAreaId.Distinct())}",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetAreaConfigDto>(cancellationToken: cancellationToken);
        return result;
    }
}