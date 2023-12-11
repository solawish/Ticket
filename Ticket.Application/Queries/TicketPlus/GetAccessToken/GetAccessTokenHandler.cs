using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Ticket.Application.Helpers;
using Ticket.Application.Options;

namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

/// <summary>
/// 登入(取得AccessToken)
/// </summary>
public class GetAccessTokenHandler : IRequestHandler<GetAccessTokenQuery, GetAccessTokenDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;
    private readonly IMD5Helper _mD5Helper;

    public GetAccessTokenHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions,
        IMD5Helper mD5Helper
        )
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
        _mD5Helper = mD5Helper;
    }

    public async Task<GetAccessTokenDto> Handle(GetAccessTokenQuery request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        request.Password = _mD5Helper.GetMd5Hash(request.Password);

        var response = await httpClient.PostAsJsonAsync(
            _ticketPlusOptions.Value.LoginUrl,
            request,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetAccessTokenDto>(cancellationToken: cancellationToken);
        return result;
    }
}