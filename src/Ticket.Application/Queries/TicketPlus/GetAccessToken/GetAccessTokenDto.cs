using Ticket.Application.Common.TicketPlus;
using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

public class GetAccessTokenDto : BaseResponse
{
    public UserInfo UserInfo { get; set; }
}