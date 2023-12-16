using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

public class GetS3ProductInfoQuery : IRequest<GetS3ProductInfoDto>
{
    /// <summary>
    /// Activity Id
    /// </summary>
    /// <example>d56972181d6e3c365b859cf9282517e1</example>
    public string ActivityId { get; set; }
}