using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ticket.Application.Commands.TicketPlus.CreateReserve;
using Ticket.Application.Commands.TicketPlus.GenerateCaptcha;
using Ticket.Application.Common.TicketPlus;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;
using Ticket.Application.Queries.TicketPlus.GetAreaConfig;
using Ticket.Application.Queries.TicketPlus.GetCaptchaAnswer;
using Ticket.Application.Queries.TicketPlus.GetProductConfig;
using Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;
using Ticket.Domain.Entities.TicketPlus;
using Ticket.Domain.Enum.TicketPlus;
using Ticket.Domain.Events.TicketPlus;

namespace Ticket.Application.Commands.TicketPlus.AutoReserve;

/// <summary>
/// 自動預約
/// </summary>
public class AutoReserveCommandHandler : IRequestHandler<AutoReserveCommand, AutoReserveDto>
{
    private readonly IMediator _mediator;
    private readonly ILogger<AutoReserveCommandHandler> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly int delayTime = 100;

    public AutoReserveCommandHandler(
        IMediator mediator,
        ILogger<AutoReserveCommandHandler> logger,
        IMemoryCache memoryCache
        )
    {
        _mediator = mediator;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<AutoReserveDto> Handle(AutoReserveCommand request, CancellationToken cancellationToken)
    {
        // 透過 ActivityId 取得S3活動資訊
        var s3ProductInfoQueryDto = _memoryCache.TryGetValue(
            string.Format(CacheKey.S3ProductInfoCacheKey, request.ActivityId),
            out GetS3ProductInfoDto cachesS3ProductInfoQueryDto)
            ? cachesS3ProductInfoQueryDto
            : await _mediator.Send(new GetS3ProductInfoQuery
            {
                ActivityId = request.ActivityId
            }, cancellationToken);
        _logger.LogInformation("GetS3ProductInfoQuery: {@s3ProductInfoQueryDto}", s3ProductInfoQueryDto);

        // 再從結果中的ProductId去取得票券的資訊
        var ticketConfigQueryDto = _memoryCache.TryGetValue(
            string.Format(CacheKey.ProductConfigCacheKey, request.ActivityId),
            out GetProductConfigDto cachesTicketConfigQueryDto)
            ? cachesTicketConfigQueryDto
            : await _mediator.Send(new GetProductConfigQuery
            {
                ProductId = s3ProductInfoQueryDto.Products.Select(x => x.ProductId)
            }, cancellationToken);
        _logger.LogInformation("GetProductConfigQuery: {@ticketConfigQueryDto}", ticketConfigQueryDto);

        // 取得票區的資訊
        var areaConfigQueryDto = _memoryCache.TryGetValue(
            string.Format(CacheKey.AreaConfigCacheKey, request.ActivityId),
            out GetAreaConfigDto cachesAreaConfigQueryDto)
            ? cachesAreaConfigQueryDto
            : await _mediator.Send(new GetAreaConfigQuery
            {
                TicketAreaId = s3ProductInfoQueryDto.Products.Where(x => string.IsNullOrEmpty(x.TicketAreaId) is false).Select(x => x.TicketAreaId)
            }, cancellationToken);
        _logger.LogInformation("GetAreaConfigQuery {@areaConfigQueryDto}", areaConfigQueryDto);

        // 取得登入的accessToken
        var accessTokenDto = _memoryCache.TryGetValue(
            string.Format(CacheKey.UserCacheKey, request.Mobile),
            out GetAccessTokenDto cacheAccessTokenDto)
            ? cacheAccessTokenDto
            : await _mediator.Send(new GetAccessTokenQuery()
            {
                CountryCode = request.CountryCode,
                Mobile = request.Mobile,
                Password = request.Password
            }, cancellationToken);
        _logger.LogInformation("GetAccessTokenQuery: {@accessTokenDto}", accessTokenDto);

        // 有沒有想要的票區
        var expectProductId = string.IsNullOrEmpty(request.AreaName) is false
            ? GetAreaProductId(areaConfigQueryDto, s3ProductInfoQueryDto, request.AreaName)
            : ticketConfigQueryDto.Result.Product.First().Id;

        // 如果要自動避開已售完的票區需產生快取
        if (request.IsCheckCount)
        {
            var processTicketCountTask = Task.Run(async () =>
            {
                while (cancellationToken.IsCancellationRequested is false)
                {
                    var newTicketConfigQueryDto = await _mediator.Send(new GetProductConfigQuery
                    {
                        ProductId = s3ProductInfoQueryDto.Products.Select(x => x.ProductId)
                    }, cancellationToken);

                    // 寫入快取
                    _memoryCache.Set(
                        string.Format(CacheKey.ProductConfigCacheKey, request.ActivityId),
                        newTicketConfigQueryDto, TimeSpan.FromHours(1));

                    var countMessage = string.Join('\n', newTicketConfigQueryDto.Result.Product.Select(x => $"{x.Id}: {x.Count}"));
                    _logger.LogInformation("重新取得票區資訊: \n{countMessage}", countMessage);

                    await Task.Delay(1000, cancellationToken);
                }
            }, cancellationToken);
        }

        // 是否需要重新產生驗證碼
        var isRegenerateCaptcha = true;
        var isPending = false;

        GenerateCaptchaDto captchaDto = null;
        GetCaptchaAnswerDto captchaCode = null;
        while (true)
        {
            // 如果cancletoken 已過期 就離開while
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("cancletoken 已過期, 使用者取消request");
                return new AutoReserveDto { CreateReserveDto = new CreateReserveDto { } };
            }

            // 是否需要重新產生驗證碼
            if (isRegenerateCaptcha)
            {
                _logger.LogInformation("需要重新產生驗證碼");

                // 產生驗證碼
                captchaDto = await _mediator.Send(new GenerateCaptchaCommand
                {
                    SessionId = ticketConfigQueryDto.Result.Product.First().SessionId,
                    Token = accessTokenDto.UserInfo.Access_token
                }, cancellationToken);
                _logger.LogInformation("GenerateCaptchaCommand");

                // 計算出驗證碼
                captchaCode = await _mediator.Send(new GetCaptchaAnswerQuery
                {
                    Data = captchaDto.Data
                }, cancellationToken);
                _logger.LogInformation("GetCaptchaAnswerQuery: {@captchaCode}", captchaCode);
            }

            // 是否要自動避開已售完的票區 因為票區資訊來源為api 避免影響到速度所以只取快取的資料 會有背景task持續取得資料並新增至快取
            if (request.IsCheckCount && isPending is false)
            {
                if (_memoryCache.TryGetValue(
                    string.Format(CacheKey.ProductConfigCacheKey, request.ActivityId),
                    out var outCacheValue))
                {
                    var reCacheTicketConfigQueryDto = outCacheValue as GetProductConfigDto;
                    var isSoldOut = reCacheTicketConfigQueryDto.Result.Product.First(x => x.Id.Equals(expectProductId)).Count <= 0;

                    // 如果是賣完了就找其他票區
                    if (isSoldOut)
                    {
                        _logger.LogInformation("{expectProductId} 這票區數量為0", expectProductId);
                        var newProduct = reCacheTicketConfigQueryDto.Result.Product.FirstOrDefault(x => x.Count > 0);
                        if (newProduct is null)
                        {
                            _logger.LogInformation("沒有其他有票的票區，使用原本的票區，ProductId: {expectProductId}", expectProductId);
                        }
                        else
                        {
                            _logger.LogInformation("找到有票的票區，ProductId: {newProduct.Id}", newProduct.Id);
                            expectProductId = newProduct.Id;
                        }
                    }
                }
            }

            // 預約票券
            var reserveResultDto = await _mediator.Send(new CreateReserveCommand
            {
                Products = new List<ReserveProduct>
                {
                    new ReserveProduct
                    {
                        ProductId = expectProductId,
                        Count = request.Count
                    }
                },
                Captcha = new Captcha
                {
                    Key = captchaDto.Key,
                    Ans = captchaCode.CaptchaAnswer
                },
                Token = accessTokenDto.UserInfo.Access_token
            }, cancellationToken);
            _logger.LogInformation("CreateReserveCommand: {@reserveResultDto}", reserveResultDto);

            if (Enum.TryParse(typeof(ReserveCodeEnum), reserveResultDto.ErrCode, out var reserveCodeEnum) is false)
            {
                isRegenerateCaptcha = false;
                isPending = false;
                _logger.LogInformation("未定義的情況，重新跑一次");
                await Task.Delay(delayTime, cancellationToken);
                continue;
            }

            switch (reserveCodeEnum)
            {
                case ReserveCodeEnum.TicketAreaLimitExceeded:
                    _logger.LogInformation("這區票賣賣完了");
                    isRegenerateCaptcha = false;
                    isPending = false;
                    await Task.Delay(delayTime, cancellationToken);
                    break;

                case ReserveCodeEnum.ProductSoldOut:
                    _logger.LogInformation("這票券賣完了");
                    isRegenerateCaptcha = false;
                    isPending = false;
                    await Task.Delay(delayTime, cancellationToken);
                    break;

                case ReserveCodeEnum.SessionSoldOut:
                    _logger.LogInformation("這場次賣完了");
                    isRegenerateCaptcha = false;
                    isPending = false;
                    await Task.Delay(delayTime, cancellationToken);
                    break;

                case ReserveCodeEnum.CaptchaFailed:
                    _logger.LogInformation("驗證碼錯誤，重新產生驗證碼");
                    isRegenerateCaptcha = true;
                    isPending = false;
                    break;

                case ReserveCodeEnum.CaptchaNotFound:
                    _logger.LogInformation("驗證碼找不到，重新產生驗證碼");
                    isRegenerateCaptcha = true;
                    isPending = false;
                    await Task.Delay(delayTime, cancellationToken);
                    break;

                case ReserveCodeEnum.Pending:
                    _logger.LogInformation("等待結果中");
                    isRegenerateCaptcha = false;
                    isPending = true;
                    await Task.Delay(delayTime, cancellationToken);
                    break;

                case ReserveCodeEnum.UserLimitExceeded:
                    _logger.LogInformation("已經有訂單了");
                    return new AutoReserveDto { CreateReserveDto = reserveResultDto };

                case ReserveCodeEnum.Success when reserveResultDto.Products.First().Status.Equals(OrderStatusEnum.RESERVED.ToString()):
                    _logger.LogInformation("預約成功");
                    await _mediator.Publish(new TicketReservedEvent
                    {
                        ActivityId = request.ActivityId,
                        Mobile = request.Mobile
                    }, cancellationToken);
                    return new AutoReserveDto { CreateReserveDto = reserveResultDto };

                default:
                    isRegenerateCaptcha = false;
                    isPending = false;
                    _logger.LogInformation("未定義的情況，重新跑一次");
                    await Task.Delay(delayTime, cancellationToken);
                    break;
            }
        }
    }

    /// <summary>
    /// 尋找匹配的票區or票名
    /// </summary>
    /// <param name="areaConfigQueryDto"></param>
    /// <param name="s3ProductInfoQueryDto"></param>
    /// <param name="areaName"></param>
    /// <returns></returns>
    private string GetAreaProductId(
        GetAreaConfigDto areaConfigQueryDto,
        GetS3ProductInfoDto s3ProductInfoQueryDto,
        string areaName)
    {
        // 先找有沒有符合的票區，有些活動可能會沒有票區(tickerArea)只有票名(productName)
        var area = areaConfigQueryDto.Result.TicketArea.FirstOrDefault(x => x.TicketAreaName.Contains(areaName));
        if (area is null)
        {
            _logger.LogInformation("找不到想要的票區，嘗試使用票名");

            var product = s3ProductInfoQueryDto.Products.FirstOrDefault(x => x.Name.Contains(areaName));
            if (product is null)
            {
                var defaultProductId = s3ProductInfoQueryDto.Products.First().ProductId;
                _logger.LogInformation("找不到想要的票名，使用第一個產品ID， ProductId: {defaultProductId}", defaultProductId);
                return defaultProductId;
            }

            _logger.LogInformation("找到想要的票名，ProductName: {Name}", product.Name);
            return product.ProductId;
        }
        var expectProductId = s3ProductInfoQueryDto.Products.FirstOrDefault(x => x.TicketAreaId.Equals(area.Id)).ProductId;
        _logger.LogInformation("找到想要的票區，AreaName: {TicketAreaName}", area.TicketAreaName);
        return expectProductId;
    }
}