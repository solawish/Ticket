using MediatR;

namespace Ticket.Application.Commands.TicketPlus.InitialActivityCache;

public class InitialActivityCacheCommand : IRequest<InitialActivityCacheDto>
{
    /// <summary>
    /// Gets or sets the activity ID.
    /// </summary>
    /// <example>d56972181d6e3c365b859cf9282517e1</example>
    public string ActivityId { get; set; }
}