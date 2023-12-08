namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

public class GetAccessTokenDto
{
    public string ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public string ErrDetail { get; set; }

    public UserInfo UserInfo { get; set; }
}