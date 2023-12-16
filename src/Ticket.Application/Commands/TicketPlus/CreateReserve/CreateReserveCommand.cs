using MediatR;
using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Commands.TicketPlus.CreateReserve;

public class CreateReserveCommand : IRequest<CreateReserveDto>
{
    /// <summary>
    /// 要預約的票券
    /// </summary>
    public IEnumerable<ReserveProduct> Products { get; set; }

    /// <summary>
    /// 驗證碼
    /// </summary>
    public Captcha Captcha { get; set; }

    public bool ReserveSeats { get; set; } = true;

    public bool ConsecutiveSeats { get; set; } = true;

    public bool FinalizedSeats { get; set; } = true;

    /// <summary>
    /// AccessToken
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyB9.naPO9ZF9q8izUu9P0wlEeoJyp4HBhCyPK9KR2NAZbuc</example>
    public string Token { get; set; }
}