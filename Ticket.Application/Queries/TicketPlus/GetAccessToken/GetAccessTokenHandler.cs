using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Ticket.Application.Options;

namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

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

        request.Password = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(request.Password))).ToLower();

        var response = await httpClient.PostAsJsonAsync(
            _ticketPlusOptions.Value.LoginUrl,
            request,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetAccessTokenDto>(cancellationToken: cancellationToken);
        return result;
    }
}