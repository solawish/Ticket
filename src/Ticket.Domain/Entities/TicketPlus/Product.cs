namespace Ticket.Domain.Entities.TicketPlus;

/// <summary>
/// S3的票券資訊
/// </summary>
public class Product
{
    public string SessionId { get; set; }

    public string TicketAreaId { get; set; }

    public string Name { get; set; }

    public int Price { get; set; }

    public bool Hidden { get; set; }

    public int SortedIndex { get; set; }

    public string ProductId { get; set; }

    public DateTime ExposeStart { get; set; }

    public DateTime ExposeEnd { get; set; }
}