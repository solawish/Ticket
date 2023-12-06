using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using Ticket.Application.Options;

namespace Ticket.Application.Command.TicketPlus;

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

        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_ticketPlusOptions.Value.ReserveUrl}")
        {
            Content = JsonContent.Create(
                request,
                options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
        };

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CreateReserveDto>(cancellationToken: cancellationToken);
        return result;
    }
}