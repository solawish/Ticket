namespace Ticket.Domain.Entities.TicketPlus;

/// <summary>
/// 預約票券
/// </summary>
public class ReserveProduct
{
    /// <summary>
    /// 票券ID
    /// </summary>
    /// <example>p000002989</example>
    public string ProductId { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    /// <example>1</example>
    public int Count { get; set; }
}