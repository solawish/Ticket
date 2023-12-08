using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetProductInfoQuery;

public class GetProductInfoQuery : IRequest<GetS3ProductInfoDto>
{
    public string ActivityId { get; set; }
}