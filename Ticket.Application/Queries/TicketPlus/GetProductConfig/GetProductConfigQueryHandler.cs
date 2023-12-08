using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Queries.TicketPlus.GetProductConfig;

/// <summary>
/// 取得票券的資訊
/// </summary>
public class GetProductConfigQueryHandler : IRequestHandler<GetProductConfigQuery, GetProductConfigDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;

    public GetProductConfigQueryHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions
        )
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
    }

    public async Task<GetProductConfigDto> Handle(GetProductConfigQuery request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        var response = await httpClient.GetAsync(
            $"{_ticketPlusOptions.Value.ConfigUrl}?productId={string.Join(',', request.ProductId)}",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetProductConfigDto>(cancellationToken: cancellationToken);
        return result;
    }
}