using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ticket.Application.Common.TicketPlus;
using Ticket.Application.Queries.TicketPlus.GetAreaConfig;
using Ticket.Application.Queries.TicketPlus.GetProductConfig;
using Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

namespace Ticket.Application.Commands.TicketPlus.InitialActivityCache;

/// <summary>
/// 初始化活動快取
/// </summary>
public class InitialActivityCacheCommandHandler : IRequestHandler<InitialActivityCacheCommand, InitialActivityCacheDto>
{
    private readonly IMediator _mediator;
    private readonly ILogger<InitialActivityCacheCommandHandler> _logger;
    private readonly IMemoryCache _memoryCache;

    public InitialActivityCacheCommandHandler(
        IMediator mediator,
        ILogger<InitialActivityCacheCommandHandler> logger,
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

        if (s3ProductInfoQueryDto.Products is null || s3ProductInfoQueryDto.Products.Any() is false)
        {
            throw new ArgumentException("找不到活動資訊");
        }
        using var s3ProductInfoQueryDtoCacheEntry = _memoryCache.CreateEntry(string.Format(CacheKey.S3ProductInfoCacheKey, request.ActivityId));
        s3ProductInfoQueryDtoCacheEntry.Value = s3ProductInfoQueryDto;
        s3ProductInfoQueryDtoCacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

        // 再從結果中的ProductId去取得票券的資訊
        var ticketConfigQueryDto = await _mediator.Send(new GetProductConfigQuery
        {
            ProductId = s3ProductInfoQueryDto.Products.Select(x => x.ProductId)
        }, cancellationToken);
        using var ticketConfigQueryDtoCacheEntry = _memoryCache.CreateEntry(string.Format(CacheKey.ProductConfigCacheKey, request.ActivityId));
        ticketConfigQueryDtoCacheEntry.Value = ticketConfigQueryDto;
        ticketConfigQueryDtoCacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

        // 取得票區的資訊
        var areaConfigQueryDto = await _mediator.Send(new GetAreaConfigQuery
        {
            TicketAreaId = s3ProductInfoQueryDto.Products.Select(x => x.TicketAreaId)
        }, cancellationToken);
        //_memoryCache.Set(string.Format(CacheKey.AreaConfigCacheKey, request.ActivityId), areaConfigQueryDto, TimeSpan.FromHours(1));
        using var areaConfigQueryDtoCacheEntry = _memoryCache.CreateEntry(string.Format(CacheKey.AreaConfigCacheKey, request.ActivityId));
        areaConfigQueryDtoCacheEntry.Value = areaConfigQueryDto;
        areaConfigQueryDtoCacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

        return new InitialActivityCacheDto();
    }
}