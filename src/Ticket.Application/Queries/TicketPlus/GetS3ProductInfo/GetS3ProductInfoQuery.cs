using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

public class GetS3ProductInfoQuery : IRequest<GetS3ProductInfoDto>
{
    /// <summary>
    /// Activity Id
    /// </summary>
    /// <example>e79352d85215d03eadb7a0618c200c85</example>
    public string ActivityId { get; set; }

    /// <summary>
    /// Session Id(場次ID)(Ex. 一場巡迴有複數天以上的場次)
    /// </summary>
    /// <example>85a3d17d6d0bf0e839e40358f5d4bada</example>
    public string SessionId { get; set; }
}