namespace Ticket.Domain.Enum;

public enum ErrorCodeEnum
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