using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetProductConfig;

public class GetProductConfigQuery : IRequest<GetProductConfigDto>
{
    /// <summary>
    /// Product Id
    /// </summary>
    public IEnumerable<string> ProductId { get; set; }
}