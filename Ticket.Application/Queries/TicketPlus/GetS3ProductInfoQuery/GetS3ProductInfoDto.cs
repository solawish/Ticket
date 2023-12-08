using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Queries.TicketPlus.GetProductInfoQuery;

public class GetS3ProductInfoDto
{
    public IEnumerable<Product> Products { get; set; }
}