using MediatR;

namespace Ticket.Application.Commands.TicketPlus.InitialActivityCache;

public class InitialActivityCacheCommand : IRequest<InitialActivityCacheDto>
{
    /// <summary>
    /// 活動Id
    /// </summary>
    public string ActivityId { get; set; }
}