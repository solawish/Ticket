using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

public class GetAccessTokenQuery : IRequest<GetAccessTokenDto>
{
    public string Mobile { get; set; }

    public string CountryCode { get; set; }

    /// <summary>
    /// 加密過的密碼
    /// </summary>
    public string Password { get; set; }
}