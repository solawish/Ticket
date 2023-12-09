namespace Ticket.Domain.Enum;

public enum ErrorCodeEnum
{
    Success = 00,
    UserLimitExceeded = 111,
    TicketAreaLimitExceeded = 121,
    CaptchaFailed = 135,
    Pending = 137
}