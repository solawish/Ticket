using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetProductConfig;

public class GetProductConfigQuery : IRequest<GetProductConfigDto>
{
    public IEnumerable<string> ProductId { get; set; }
}