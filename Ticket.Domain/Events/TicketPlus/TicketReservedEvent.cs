using MediatR;

namespace Ticket.Domain.Events.TicketPlus;

public class TicketReservedEvent : INotification
{
    public string ActivityId { get; set; }

    public string Mobile { get; set; }
}