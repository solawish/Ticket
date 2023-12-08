using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

public class GetS3ProductInfoQuery : IRequest<GetS3ProductInfoDto>
{
    public string ActivityId { get; set; }
}