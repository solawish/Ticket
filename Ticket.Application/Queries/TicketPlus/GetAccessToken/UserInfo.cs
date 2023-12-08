namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

public class UserInfo
{
    public string Id { get; set; }

    public string Access_token { get; set; }

    public string Refresh_token { get; set; }

    public int Access_token_expires_in { get; set; }

    public bool VerifyEmail { get; set; }
}