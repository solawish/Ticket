using MediatR;

namespace Ticket.Application.Commands.TicketPlus.GenerateRecaptchCommand;

public class GenerateCaptchaCommand : IRequest<GenerateCaptchaDto>
{
    public string SessionId { get; set; }

    public bool Refresh { get; set; }

    public string Token { get; set; }
}