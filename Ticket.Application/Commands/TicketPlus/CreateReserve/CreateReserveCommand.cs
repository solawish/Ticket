using MediatR;
using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Commands.TicketPlus.CreateReserve;

public class CreateReserveCommand : IRequest<CreateReserveDto>
{
    public IEnumerable<ReserveProduct> Products { get; set; }

    public Captcha Captcha { get; set; }

    public bool ReserveSeats { get; set; } = true;

    public bool ConsecutiveSeats { get; set; } = true;

    public bool FinalizedSeats { get; set; } = true;

    public string Token { get; set; }
}