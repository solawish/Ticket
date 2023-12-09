using MediatR;
using Microsoft.Extensions.Logging;
using Ticket.Domain.Events.TicketPlus;

namespace Ticket.Application.DomainEventHandlers.TicketPlus;

/// <summary>
/// 預約成功事件
/// </summary>
public class TicketReservedEventHandler : INotificationHandler<TicketReservedEvent>
{
    private readonly ILogger<TicketReservedEventHandler> _logger;

    public TicketReservedEventHandler(
        ILogger<TicketReservedEventHandler> logger
        )
    {
        _logger = logger;
    }

    public async Task Handle(TicketReservedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(notification.ActivityId)}:{notification.ActivityId}," +
            $" {nameof(notification.Mobile)}:{notification.Mobile} 預約成功");
    }
}