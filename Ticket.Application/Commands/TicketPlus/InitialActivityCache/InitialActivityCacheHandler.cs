﻿using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ticket.Application.Common;
using Ticket.Application.Queries.TicketPlus.GetProductConfig;
using Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

namespace Ticket.Application.Commands.TicketPlus.InitialActivityCache;

/// <summary>
/// 初始化活動快取
/// </summary>
public class InitialActivityCacheHandler : IRequestHandler<InitialActivityCacheCommand, InitialActivityCacheDto>
{
    private readonly IMediator _mediator;
    private readonly ILogger<InitialActivityCacheHandler> _logger;
    private readonly IMemoryCache _memoryCache;

    public InitialActivityCacheHandler(
        IMediator mediator,
        ILogger<InitialActivityCacheHandler> logger,
        IMemoryCache memoryCache
        )
    {
        _mediator = mediator;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<InitialActivityCacheDto> Handle(InitialActivityCacheCommand request, CancellationToken cancellationToken)
    {
        // 透過 ActivityId 取得S3活動資訊
        var s3ProductInfoQueryDto = await _mediator.Send(new GetS3ProductInfoQuery
        {
            ActivityId = request.ActivityId
        }, cancellationToken);

        // 將資料寫入memoryCache
        _memoryCache.Set(string.Format(Const.S3ProductInfoCacheKey, request.ActivityId), s3ProductInfoQueryDto);

        // 再從結果中的ProductId去取得票券的資訊
        var ticketConfigQueryDto = await _mediator.Send(new GetProductConfigQuery
        {
            ProductId = s3ProductInfoQueryDto.Products.Select(x => x.ProductId)
        }, cancellationToken);
        _memoryCache.Set(string.Format(Const.ProductConfigCacheKey, request.ActivityId), ticketConfigQueryDto);

        return new InitialActivityCacheDto();
    }
}