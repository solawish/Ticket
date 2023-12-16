using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Commands.TicketPlus.CreateReserve;

/// <summary>
/// 預約票券
/// </summary>
public class CreateReserveCommandHandler : IRequestHandler<CreateReserveCommand, CreateReserveDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TicketPlusOptions> _ticketPlusOptions;

    public CreateReserveCommandHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketPlusOptions> ticketPlusOptions)
    {
        _httpClientFactory = httpClientFactory;
        _ticketPlusOptions = ticketPlusOptions;
    }

    public async Task<CreateReserveDto> Handle(
        CreateReserveCommand request,
        CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(_ticketPlusOptions.Value.Name);

        // httpclient add header
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Token}");

        var response = await httpClient.PostAsJsonAsync(
          _ticketPlusOptions.Value.ReserveUrl,
          request,
          cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CreateReserveDto>(cancellationToken: cancellationToken);
        return result;
    }
}