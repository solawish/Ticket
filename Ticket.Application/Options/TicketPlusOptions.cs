namespace Ticket.Application.Options;

public class TicketPlusOptions
{
    public string Name { get; set; }

    public string S3ConfigUrl { get; set; }

    public string ReserveUrl { get; set; }

    public string GenerateCaptchaUrl { get; set; }

    public string LoginUrl { get; set; }
}