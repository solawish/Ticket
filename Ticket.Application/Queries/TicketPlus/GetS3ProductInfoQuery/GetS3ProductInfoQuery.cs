using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetProductInfoQuery;

public class GetS3ProductInfoQuery : IRequest<GetS3ProductInfoDto>
{
    public string ActivityId { get; set; }
}