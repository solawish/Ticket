namespace Ticket.Domain.Entities.TicketPlus;

public class OrderProduct
{
    //{
    //            "idx": 6319745,
    //            "userId": "fetix.1698336036062729",
    //            "userType": "oone",
    //            "productId": "p000002938",
    //            "info": {},
    //            "count": 1,
    //            "status": "RESERVED",
    //            "orderId": 6308540,
    //            "hash": "4dc400b4da1345907db1f8da2c18d4c5",
    //            "expiryTimestamp": "2023-12-08T14:53:05+08:00",
    //            "createdAt": "2023-12-08T14:43:05+08:00"
    //        }
    public int Idx { get; set; }

    public string UserId { get; set; }

    public string UserType { get; set; }

    public string ProductId { get; set; }

    public object Info { get; set; }

    public int Count { get; set; }

    public string Status { get; set; }

    public int OrderId { get; set; }

    public string Hash { get; set; }

    public DateTime ExpiryTimestamp { get; set; }

    public DateTime CreatedAt { get; set; }
}