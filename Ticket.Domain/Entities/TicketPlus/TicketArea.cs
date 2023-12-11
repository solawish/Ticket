namespace Ticket.Domain.Entities.TicketPlus;

public class TicketArea
{
    //    {
    //    "saleStart": "2023-12-09T11:00:00+08:00",
    //    "ticketAreaName": "1F搖滾B區",
    //    "ticketAreaLimit": true,
    //    "exposeStart": "2023-11-20T23:55:00+08:00",
    //    "saleEnd": "2024-02-18T17:00:00+08:00",
    //    "status": "pending",
    //    "createdAt": "2023-11-20T23:48:17+08:00",
    //    "exposeEnd": "2024-02-19T00:00:00+08:00",
    //    "sortedIndex": 2,
    //    "seatAssignment": true,
    //    "updatedAt": "2023-12-01T10:51:34+08:00",
    //    "userLimit": 4,
    //    "virtualSeat": true,
    //    "id": "a000000874",
    //    "price": 2400,
    //    "hidden": false
    //}
    public DateTime SaleStart { get; set; }

    public string TicketAreaName { get; set; }

    public bool TicketAreaLimit { get; set; }

    public DateTime ExposeStart { get; set; }

    public DateTime SaleEnd { get; set; }

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExposeEnd { get; set; }

    public int SortedIndex { get; set; }

    public bool SeatAssignment { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int UserLimit { get; set; }

    public bool VirtualSeat { get; set; }

    public string Id { get; set; }

    public int Price { get; set; }

    public bool Hidden { get; set; }
}