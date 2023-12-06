using MediatR;

namespace Ticket.Application.Commands.TicketPlus.GenerateRecaptchCommand;

public class GenerateRecaptchCommand : IRequest<GenerateRecaptchDto>
{
    public string SessionId { get; set; }

    public bool Refresh { get; set; }

    public string Token { get; set; }
}