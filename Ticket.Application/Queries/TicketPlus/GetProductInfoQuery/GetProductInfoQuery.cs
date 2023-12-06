using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetProductInfoQuery;

public class GetProductInfoQuery : IRequest<GetProductInfoDto>
{
    public string ActivityId { get; set; }
}