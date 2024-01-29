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
public class CreateAutoReserveCommandHandler : IRequestHandler<CreateAutoReserveCommand, CreateAutoReserveDto>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateAutoReserveCommandHandler> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly int delayTime = 400;
    private readonly int pendingDelayTime = 5000;
    private readonly int checkCountTime = 1000;

    public CreateAutoReserveCommandHandler(
        IMediator mediator,
        ILogger<CreateAutoReserveCommandHandler> logger,
        IMemoryCache memoryCache
        )
    {
        _mediator = mediator;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<CreateAutoReserveDto> Handle(CreateAutoReserveCommand request, CancellationToken cancellationToken)
    {
        // 透過 ActivityId 取得S3特定場次活動資訊
        var s3ProductInfoQueryDto = _memoryCache.TryGetValue(
            string.Format(CacheKey.S3ProductInfoCacheKey, request.ActivityId),
            out GetS3ProductInfoDto cacheS3ProductInfoQueryDto)
            ? cacheS3ProductInfoQueryDto
            : await _mediator.Send(new GetS3ProductInfoQuery
            {
                ActivityId = request.ActivityId,
                SessionId = request.SessionId
            }, cancellationToken);
        if (s3ProductInfoQueryDto.Products is null || s3ProductInfoQueryDto.Products.Any() is false)
        {
            throw new ArgumentException("找不到活動資訊");
        }
        _logger.LogInformation("{GetS3ProductInfoQuery}: {@s3ProductInfoQueryDto}", nameof(GetS3ProductInfoQuery), s3ProductInfoQueryDto);

        // 再從結果中的ProductId去取得票券的資訊
        var productConfigQueryDto = _memoryCache.TryGetValue(
            string.Format(CacheKey.ProductConfigCacheKey, request.ActivityId),
            out GetProductConfigDto cacheProductConfigQueryDto)
            ? cacheProductConfigQueryDto
            : await _mediator.Send(new GetProductConfigQuery
            {
                ProductId = s3ProductInfoQueryDto.Products.Select(x => x.ProductId)
            }, cancellationToken);
        _logger.LogInformation("{GetProductConfigQuery}: {@productConfigQueryDto}", nameof(GetProductConfigQuery), productConfigQueryDto);

        // 票區資訊來源
        var ticketCountSource = s3ProductInfoQueryDto.Products.Any(x => string.IsNullOrEmpty(x.TicketAreaId) is false)
            ? TicketCountSource.Area
            : TicketCountSource.Product;

        // 取得票區的資訊
        var areaConfigQueryDto = _memoryCache.TryGetValue(
            string.Format(CacheKey.AreaConfigCacheKey, request.ActivityId),
            out GetAreaConfigDto cacheAreaConfigQueryDto)
            ? cacheAreaConfigQueryDto
            : await _mediator.Send(new GetAreaConfigQuery
            {
                TicketAreaId = s3ProductInfoQueryDto.Products.Where(x => string.IsNullOrEmpty(x.TicketAreaId) is false).Select(x => x.TicketAreaId)
            }, cancellationToken);
        _logger.LogInformation("{GetAreaConfigQuery}: {@areaConfigQueryDto}", nameof(GetAreaConfigQuery), areaConfigQueryDto);

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
        if (accessTokenDto.UserInfo is null)
        {
            throw new ArgumentException("登入失敗");
        }
        _logger.LogInformation("{GetAccessTokenQuery}: {@accessTokenDto}", nameof(GetAccessTokenQuery), accessTokenDto);

        // 有沒有想要的票區
        var expectProductId = string.IsNullOrEmpty(request.AreaName) is false
            ? GetAreaProductId(areaConfigQueryDto, s3ProductInfoQueryDto, request.AreaName, ticketCountSource)
            : productConfigQueryDto.Result.Product.First().Id;

        // 如果要自動避開已售完的票區需產生快取
        if (request.IsCheckCount)
        {
            RunProcessTicketCountTask(ticketCountSource, s3ProductInfoQueryDto, request.ActivityId, cancellationToken);
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
                return new CreateAutoReserveDto { CreateReserveDto = new CreateReserveDto { } };
            }

            // 是否需要重新產生驗證碼
            if (isRegenerateCaptcha)
            {
                _logger.LogInformation("需要重新產生驗證碼");

                // 產生驗證碼
                captchaDto = await _mediator.Send(new GenerateCaptchaCommand
                {
                    SessionId = productConfigQueryDto.Result.Product.First().SessionId,
                    Token = accessTokenDto.UserInfo.Access_token
                }, cancellationToken);
                _logger.LogInformation("{GenerateCaptchaCommand}", nameof(GenerateCaptchaCommand));

                // 計算出驗證碼
                captchaCode = await _mediator.Send(new GetCaptchaAnswerQuery
                {
                    Data = captchaDto.Data
                }, cancellationToken);
                _logger.LogInformation("{GetCaptchaAnswerQuery}: {@captchaCode}", nameof(GetCaptchaAnswerQuery), captchaCode);
            }

            // 是否要自動避開已售完的票區 因為票區資訊來源為api 避免影響到速度所以只取快取的資料 會有背景task持續取得資料並新增至快取
            if (request.IsCheckCount && isPending is false)
            {
                switch (ticketCountSource)
                {
                    case TicketCountSource.Area:
                        if (_memoryCache.TryGetValue(
                            string.Format(CacheKey.AreaConfigCacheKey, request.ActivityId),
                            out var outAreaConfigCacheValue))
                        {
                            var reCacheAreaConfigQueryDto = outAreaConfigCacheValue as GetAreaConfigDto;
                            var ticketAreaId = s3ProductInfoQueryDto.Products.First(x => x.ProductId.Equals(expectProductId)).TicketAreaId;
                            var isSoldOut = reCacheAreaConfigQueryDto.Result.TicketArea.First(x => x.Id.Equals(ticketAreaId)).Count <= 0;

                            // 如果是賣完了就找其他票區
                            if (isSoldOut)
                            {
                                _logger.LogInformation("{expectProductId} 這票區數量為 0", expectProductId);
                                var newArea = reCacheAreaConfigQueryDto.Result.TicketArea
                                    .Where(x => x.Count > 0)
                                    .OrderByDescending(x => x.Count);
                                if (newArea.Any() is false)
                                {
                                    _logger.LogInformation("沒有其他有票的票區，使用原本的票區，ProductId: {expectProductId}", expectProductId);
                                }
                                else
                                {
                                    var newProductId = s3ProductInfoQueryDto.Products.First(x => x.TicketAreaId.Equals(newArea.First().Id)).ProductId;
                                    _logger.LogInformation("找到有票的票區，ProductId: {newProductId}", newProductId);
                                    expectProductId = newProductId;
                                }
                            }
                        }
                        break;

                    case TicketCountSource.Product:
                        if (_memoryCache.TryGetValue(
                            string.Format(CacheKey.ProductConfigCacheKey, request.ActivityId),
                            out var outProductCacheValue))
                        {
                            var reCacheProductConfigQueryDto = outProductCacheValue as GetProductConfigDto;
                            var isSoldOut = reCacheProductConfigQueryDto.Result.Product.First(x => x.Id.Equals(expectProductId)).Count <= 0;

                            // 如果是賣完了就找其他票區
                            if (isSoldOut)
                            {
                                _logger.LogInformation("{expectProductId} 這票區數量為 0", expectProductId);
                                var newProduct = reCacheProductConfigQueryDto.Result.Product
                                    .Where(x => x.Count > 0)
                                    .OrderByDescending(x => x.Count);
                                if (newProduct.Any() is false)
                                {
                                    _logger.LogInformation("沒有其他有票的票區，使用原本的票區，ProductId: {expectProductId}", expectProductId);
                                }
                                else
                                {
                                    var newProductId = newProduct.First().Id;
                                    _logger.LogInformation("找到有票的票區，ProductId: {newProductId}", newProductId);
                                    expectProductId = newProductId;
                                }
                            }
                        }
                        break;

                    default:
                        throw new ArgumentException("未定義的票區資訊來源");
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
            _logger.LogInformation("{CreateReserveCommand}: {@reserveResultDto}", nameof(CreateReserveCommand), reserveResultDto);

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
                    var waitSecond = reserveResultDto.WaitSecond.Equals(default) ? pendingDelayTime : reserveResultDto.WaitSecond * 1000;
                    await Task.Delay(waitSecond, cancellationToken);
                    break;

                case ReserveCodeEnum.UserLimitExceeded:
                    _logger.LogInformation("已經有訂單了");
                    return new CreateAutoReserveDto { CreateReserveDto = reserveResultDto };

                case ReserveCodeEnum.Success when reserveResultDto.Products.First().Status.Equals(OrderStatusEnum.RESERVED.ToString()):
                    _logger.LogInformation("預約成功");
                    await _mediator.Publish(new TicketReservedEvent
                    {
                        ActivityId = request.ActivityId,
                        Mobile = request.Mobile
                    }, cancellationToken);
                    return new CreateAutoReserveDto { CreateReserveDto = reserveResultDto };

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
    /// <param name="ticketCountSource"></param>
    /// <returns></returns>
    private string GetAreaProductId(
        GetAreaConfigDto areaConfigQueryDto,
        GetS3ProductInfoDto s3ProductInfoQueryDto,
        string areaName,
        TicketCountSource ticketCountSource)
    {
        switch (ticketCountSource)
        {
            case TicketCountSource.Area:
                var areaResult = areaConfigQueryDto.Result.TicketArea.Where(x => x.TicketAreaName.Contains(areaName));
                if (areaResult.Any())
                {
                    _logger.LogInformation("找到想要的票區，AreaName: {TicketAreaName}", areaResult.First().TicketAreaName);
                    return s3ProductInfoQueryDto.Products.FirstOrDefault(x => x.TicketAreaId.Equals(areaResult.First().Id)).ProductId;
                }
                break;

            case TicketCountSource.Product:
                var productResult = s3ProductInfoQueryDto.Products.Where(x => x.Name.Contains(areaName));
                if (productResult.Any())
                {
                    _logger.LogInformation("找到想要的票名，ProductName: {Name}", productResult.First().Name);
                    return productResult.First().ProductId;
                }
                break;

            default:
                throw new ArgumentException("未定義的票區資訊來源");
        }

        var defaultProductId = s3ProductInfoQueryDto.Products.First().ProductId;
        _logger.LogInformation("找不到想要的票名，使用第一個產品ID， ProductId: {defaultProductId}", defaultProductId);
        return defaultProductId;
    }

    /// <summary>
    /// 執行定期取得票數的Task
    /// </summary>
    /// <param name="ticketCountSource"></param>
    /// <param name="s3ProductInfoQueryDto"></param>
    /// <param name="activityId"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentException"></exception>
    private void RunProcessTicketCountTask(
        TicketCountSource ticketCountSource,
        GetS3ProductInfoDto s3ProductInfoQueryDto,
        string activityId,
        CancellationToken cancellationToken
        )
    {
        var processTicketCountTask = Task.Run(async () =>
        {
            while (cancellationToken.IsCancellationRequested is false)
            {
                switch (ticketCountSource)
                {
                    case TicketCountSource.Area:
                        var newAreaConfigQueryDto = await _mediator.Send(new GetAreaConfigQuery
                        {
                            TicketAreaId = s3ProductInfoQueryDto.Products.Where(x => string.IsNullOrEmpty(x.TicketAreaId) is false).Select(x => x.TicketAreaId)
                        }, cancellationToken);

                        // 寫入快取
                        _memoryCache.Set(
                            string.Format(CacheKey.AreaConfigCacheKey, activityId),
                            newAreaConfigQueryDto, TimeSpan.FromHours(1));

                        var areaCountResult = newAreaConfigQueryDto.Result.TicketArea.Select(x => new { x.Id, x.Count });
                        _logger.LogInformation("重新取得票區票數資訊: {@countMessage}", areaCountResult);
                        break;

                    case TicketCountSource.Product:
                        var newProductConfigQueryDto = await _mediator.Send(new GetProductConfigQuery
                        {
                            ProductId = s3ProductInfoQueryDto.Products.Select(x => x.ProductId)
                        }, cancellationToken);

                        // 寫入快取
                        _memoryCache.Set(
                            string.Format(CacheKey.ProductConfigCacheKey, activityId),
                            newProductConfigQueryDto, TimeSpan.FromHours(1));

                        var productCountResult = newProductConfigQueryDto.Result.Product.Select(x => new { x.Id, x.Count });
                        _logger.LogInformation("重新取得票區票數資訊: {@countMessage}", productCountResult);
                        break;

                    default:
                        throw new ArgumentException("未定義的票區資訊來源");
                }

                await Task.Delay(checkCountTime, cancellationToken);
            }
        }, cancellationToken);
    }
}