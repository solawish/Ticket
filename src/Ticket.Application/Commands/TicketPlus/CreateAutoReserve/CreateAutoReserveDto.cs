using Ticket.Application.Commands.TicketPlus.CreateReserve;

namespace Ticket.Application.Commands.TicketPlus.AutoReserve;

public class CreateAutoReserveDto
{
    /// <summary>
    /// 預約的結果
    /// </summary>
    public CreateReserveDto CreateReserveDto { get; set; }
}