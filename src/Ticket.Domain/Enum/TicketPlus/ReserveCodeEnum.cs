namespace Ticket.Domain.Enum.TicketPlus;

/// <summary>
/// 訂閱票券回傳代碼
/// </summary>
public enum ReserveCodeEnum
{
    Success = 00,
    UserLimitExceeded = 111,
    ProductSoldOut = 113,
    SessionSoldOut = 115,
    TicketAreaLimitExceeded = 121,
    CaptchaFailed = 135,
    CaptchaNotFound = 136,
    Pending = 137
}