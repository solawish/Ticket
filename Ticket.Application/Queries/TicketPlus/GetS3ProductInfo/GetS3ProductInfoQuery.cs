using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

public class GetS3ProductInfoQuery : IRequest<GetS3ProductInfoDto>
{
    /// <summary>
    /// Activity Id
    /// </summary>
    public string ActivityId { get; set; }
}