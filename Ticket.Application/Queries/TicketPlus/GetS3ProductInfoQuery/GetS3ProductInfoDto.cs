using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Queries.TicketPlus.GetProductInfoQuery;

public class GetProductInfoDto
{
    public IEnumerable<Product> Products { get; set; }
}