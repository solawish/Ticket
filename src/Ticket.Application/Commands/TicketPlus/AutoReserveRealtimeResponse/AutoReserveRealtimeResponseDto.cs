using Ticket.Application.Commands.TicketPlus.CreateReserve;

namespace Ticket.Application.Commands.TicketPlus.AutoReserveRealtimeResponse;

public class AutoReserveRealtimeResponseDto
{
    /// <summary>
    /// 預約的結果
    /// </summary>
    public CreateReserveDto CreateReserveDto { get; set; }
}