﻿using MediatR;

namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

public class GetAccessTokenQuery : IRequest<GetAccessTokenDto>
{
    /// <summary>
    /// 登入的手機號碼
    /// </summary>
    /// <example>0912345678</example>
    public string Mobile { get; set; }

    /// <summary>
    /// 登入的國碼
    /// </summary>
    /// <example>886</example>
    public string CountryCode { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; }
}