using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetCaptchaAnswerQuery;

public class GetCaptchaAnswerQuery : IRequest<GetCaptchaAnswerDto>
{
    /// <summary>
    /// SVG Data
    /// </summary>
    public string Data { get; set; }
}