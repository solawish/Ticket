namespace Ticket.Domain.Enum;

public enum ErrorCodeEnum
{
    Success = 00,
    UserLimitExceeded = 111,
    CaptchaFailed = 135,
    Pending = 137
}