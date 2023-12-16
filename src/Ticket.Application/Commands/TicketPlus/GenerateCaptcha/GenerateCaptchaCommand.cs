using MediatR;

namespace Ticket.Application.Commands.TicketPlus.GenerateCaptcha;

public class GenerateCaptchaCommand : IRequest<GenerateCaptchaDto>
{
    /// <summary>
    /// 屬於特定票券的SessionId
    /// </summary>
    /// <example>s000000448</example>
    public string SessionId { get; set; }

    /// <summary>
    /// 是否要重新產生驗證碼
    /// </summary>
    /// <example>false</example>
    public bool Refresh { get; set; }

    /// <summary>
    /// AccessToken
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyB9.naPO9ZF9q8izUu9P0wlEeoJyp4HBhCyPK9KR2NAZbuc</example>
    public string Token { get; set; }
}