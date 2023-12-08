using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

/// <summary>
/// 取得S3活動資訊
/// </summary>
public class GetS3ProductInfoQueryHandler : IRequestHandler<GetS3ProductInfoQuery, GetS3ProductInfoDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;

    public GetS3ProductInfoQueryHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions)
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
    }

    public async Task<GetS3ProductInfoDto> Handle(GetS3ProductInfoQuery request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        // todo : this pathArgu should add some options (ex. products, sessions, event ...etc )
        var pahtArgu = $"event/{request.ActivityId}/products.json";
        var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_ticketPlusOptions.Value.S3ConfigUrl}?path={pahtArgu}");

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetS3ProductInfoDto>(cancellationToken: cancellationToken);
        return result;
    }
}