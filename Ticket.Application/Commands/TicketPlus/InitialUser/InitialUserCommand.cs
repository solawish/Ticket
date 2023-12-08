using MediatR;

namespace Ticket.Application.Commands.TicketPlus.InitialUser;

public class InitialUserCommand : IRequest<InitialUserDto>
{
    public string Mobile { get; set; }

    public string CountryCode { get; set; }

    public string Password { get; set; }
}