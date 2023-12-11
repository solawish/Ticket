namespace Ticket.Domain.Entities.TicketPlus;

public class ProductConfig
{
    //{
    //                "unit": 1,
    //                "disabilitiesType": 0,
    //                "saleEnd": "2023-12-30T23:59:00+08:00",
    //                "status": "onsale",
    //                "sortedIndex": 1,
    //                "coupon": [],
    //                "uncountable": false,
    //                "productIbonSaleStart": "2023-12-02T12:00:00+08:00",
    //                "id": "p000002938",
    //                "eventId": "e000000284",
    //                "saleStart": "2023-12-02T12:00:00+08:00",
    //                "productIbon": true,
    //                "productIbonSaleEnd": "2023-12-30T23:59:00+08:00",
    //                "productLimit": false,
    //                "ccDiscount": [],
    //                "exposeStart": "2023-11-27T18:15:00+08:00",
    //                "createdAt": "2023-11-27T18:14:14+08:00",
    //                "exposeEnd": "2024-01-02T00:00:00+08:00",
    //                "seatAssignment": false,
    //                "sessionId": "s000000442",
    //                "updatedAt": "2023-11-27T18:17:46+08:00",
    //                "userLimit": 0,
    //                "price": 750,
    //                "purchaseLimit": 4,
    //                "autoRelease": true,
    //                "hidden": false,
    //                "count": 0
    //            }

    public int Unit { get; set; }

    public int DisabilitiesType { get; set; }

    public DateTime SaleEnd { get; set; }

    public string Status { get; set; }

    public int SortedIndex { get; set; }

    public List<object> Coupon { get; set; }

    public bool Uncountable { get; set; }

    public DateTime ProductIbonSaleStart { get; set; }

    public string Id { get; set; }

    public string EventId { get; set; }

    public DateTime SaleStart { get; set; }

    public bool ProductIbon { get; set; }

    public DateTime ProductIbonSaleEnd { get; set; }

    public bool ProductLimit { get; set; }

    public List<object> CcDiscount { get; set; }

    public DateTime ExposeStart { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExposeEnd { get; set; }

    public bool SeatAssignment { get; set; }

    public string SessionId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int UserLimit { get; set; }

    public int Price { get; set; }

    public int PurchaseLimit { get; set; }

    public bool AutoRelease { get; set; }

    public bool Hidden { get; set; }

    public int Count { get; set; }
}