namespace Ticket.Domain.Entities.TicketPlus;

/// <summary>
/// 驗證碼
/// </summary>
public class Captcha
{
    /// <summary>
    /// 驗證碼的Key (目前看起來像是遠大的會員ID)
    /// </summary>
    /// <example>fetix.1128536736962249</example>
    public string Key { get; set; }

    /// <summary>
    /// 驗證碼的答案
    /// </summary>
    /// <example>abcd</example>
    public string Ans { get; set; }
}