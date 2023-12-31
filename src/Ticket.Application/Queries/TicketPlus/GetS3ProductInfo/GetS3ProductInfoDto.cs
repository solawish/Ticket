﻿using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

public class GetS3ProductInfoDto
{
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
}