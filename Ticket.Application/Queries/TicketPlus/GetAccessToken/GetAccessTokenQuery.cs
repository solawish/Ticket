using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

public class GetAccessTokenQuery : IRequest<GetAccessTokenDto>
{
    public string Mobile { get; set; }

    public string CountryCode { get; set; }

    public string Password { get; set; }
}