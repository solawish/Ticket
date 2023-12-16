using MediatR;

namespace Ticket.Domain.Events.TicketPlus;

/// <summary>
/// 票券預約成功的事件
/// </summary>
public class TicketReservedEvent : INotification
{
    /// <summary>
    /// 場次
    /// </summary>
    public string ActivityId { get; set; }

    /// <summary>
    /// 手機號碼
    /// </summary>
    public string Mobile { get; set; }
}