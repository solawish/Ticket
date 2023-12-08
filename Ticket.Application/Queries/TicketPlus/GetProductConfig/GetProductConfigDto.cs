namespace Ticket.Application.Queries.TicketPlus.GetProductConfig;

public class GetProductConfigDto
{
    //    {
    //    "errCode": "00",
    //    "errMsg": "",
    //    "errDetail": "",
    //    "result": {
    //        "product": [
    //            {
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
    //        ]
    //    }
    //}
    public string ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public string ErrDetail { get; set; }

    public Result Result { get; set; }
}

public class Result
{
    public List<ProductConfig> Product { get; set; }
}