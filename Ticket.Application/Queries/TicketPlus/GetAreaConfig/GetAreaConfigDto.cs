namespace Ticket.Application.Queries.TicketPlus.GetAreaConfig;

public class GetAreaConfigDto
{
    public string ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public string ErrDetail { get; set; }

    public AreaConfiResult Result { get; set; }
}

public class AreaConfiResult
{
    public List<TicketArea> TicketArea { get; set; } = new List<TicketArea>();
}