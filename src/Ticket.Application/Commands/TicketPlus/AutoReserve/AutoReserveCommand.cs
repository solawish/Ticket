using MediatR;

namespace Ticket.Application.Commands.TicketPlus.AutoReserve;

/// <summary>
/// AutoReserveCommand
/// </summary>
public class AutoReserveCommand : IRequest<AutoReserveDto>
{
    /// <summary>
    /// 活動Id
    /// </summary>
    /// <example>e79352d85215d03eadb7a0618c200c85</example>
    public string ActivityId { get; set; }

    /// <summary>
    /// Session Id(場次ID)(Ex. 一場巡迴有複數天以上的場次)
    /// </summary>
    /// <example>85a3d17d6d0bf0e839e40358f5d4bada</example>
    public string SessionId { get; set; }

    /// <summary>
    /// 登入的手機號碼
    /// </summary>
    /// <example>0912345678</example>
    public string Mobile { get; set; }

    /// <summary>
    /// 登入的國碼
    /// </summary>
    /// <example>886</example>
    public string CountryCode { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    /// <example>123456</example>
    public string Password { get; set; }

    /// <summary>
    /// 購買數量
    /// </summary>
    /// <example>1</example>
    public int Count { get; set; }

    /// <summary>
    /// 想要的票區關鍵字
    /// </summary>
    /// <example>VIP</example>
    public string AreaName { get; set; }

    /// <summary>
    /// 是否要自動略過已售完的票區
    /// </summary>
    /// <example>true</example>
    public bool IsCheckCount { get; set; }
}