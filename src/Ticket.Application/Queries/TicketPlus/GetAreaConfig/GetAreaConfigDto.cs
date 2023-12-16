using Ticket.Application.Common.TicketPlus;
using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Queries.TicketPlus.GetAreaConfig;

public class GetAreaConfigDto : BaseResponse
{
    public AreaConfiResult Result { get; set; } = new AreaConfiResult();
}

public class AreaConfiResult
{
    public List<TicketArea> TicketArea { get; set; } = new List<TicketArea>();
}