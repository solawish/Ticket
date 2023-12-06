using MediatR;
using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Commands.TicketPlus;

public class CreateReserveCommand : IRequest<CreateReserveDto>
{
    public IEnumerable<ReserveProduct> Products { get; set; }

    public Captcha Captcha { get; set; }

    public bool ReserveSeats { get; set; }

    public bool ConsecutiveSeats { get; set; }

    public bool FinalizedSeats { get; set; }
}