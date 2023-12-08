using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Queries.TicketPlus.GetAccessTokenQuery;

/// <summary>
/// 登入(取得AccessToken)
/// </summary>
public class GetAccessTokenHandler : IRequestHandler<GetAccessTokenQuery, GetAccessTokenDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;

    public GetAccessTokenHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions
        )
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
    }

    public async Task<GetAccessTokenDto> Handle(GetAccessTokenQuery request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        var response = await httpClient.PostAsJsonAsync(
            _ticketPlusOptions.Value.LoginUrl,
            request,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetAccessTokenDto>(cancellationToken: cancellationToken);
        return result;
    }
}