using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetCaptchaAnswer;

public class GetCaptchaAnswerQuery : IRequest<GetCaptchaAnswerDto>
{
    /// <summary>
    /// SVG Data
    /// </summary>
    public string Data { get; set; }
}