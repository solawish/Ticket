using MediatR;

namespace Ticket.Application.Commands.TicketPlus.AutoReserve;

public class AutoReserveCommand : IRequest<AutoReserveDto>
{
    /// <summary>
    /// 活動Id
    /// </summary>
    public string ActivityId { get; set; }

    /// <summary>
    /// 登入的手機號碼
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// 登入的國碼 (Ex. 886)
    /// </summary>
    public string CountryCode { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 購買數量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 想要的票區關鍵字
    /// </summary>
    public string AreaName { get; set; }

    /// <summary>
    /// 是否要自動略過已售完的票區
    /// </summary>
    public bool IsCheckCount { get; set; }
}