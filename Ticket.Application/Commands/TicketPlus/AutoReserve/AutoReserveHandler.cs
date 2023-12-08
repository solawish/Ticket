﻿using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Ticket.Application.Commands.TicketPlus.CreateReserve;
using Ticket.Application.Commands.TicketPlus.GenerateCaptcha;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;
using Ticket.Application.Queries.TicketPlus.GetCaptchaAnswer;
using Ticket.Application.Queries.TicketPlus.GetProductConfig;
using Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;
using Ticket.Domain.Entities.TicketPlus;
using Ticket.Domain.Enum;

namespace Ticket.Application.Commands.TicketPlus.AutoReserve;

/// <summary>
/// 自動預約
/// </summary>
public class AutoReserveHandler : IRequestHandler<AutoReserveCommand, AutoReserveDto>
{
    private readonly IMediator _mediator;
    private readonly ILogger<AutoReserveHandler> _logger;

    public AutoReserveHandler(
        IMediator mediator,
        ILogger<AutoReserveHandler> logger
        )
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<AutoReserveDto> Handle(AutoReserveCommand request, CancellationToken cancellationToken)
    {
        // todo: 有些固定內容要加CACHE

        // 透過 ActivityId 取得S3活動資訊
        var s3ProductInfoQueryDto = await _mediator.Send(new GetS3ProductInfoQuery
        {
            ActivityId = request.ActivityId
        }, cancellationToken);
        _logger.LogInformation($"GetS3ProductInfoQuery: {JsonSerializer.Serialize(s3ProductInfoQueryDto)}");

        // 再從結果中的ProductId去取得票券的資訊
        var ticketConfigQueryDto = await _mediator.Send(new GetProductConfigQuery
        {
            ProductId = s3ProductInfoQueryDto.Products.Select(x => x.ProductId)
        }, cancellationToken);
        _logger.LogInformation($"GetProductConfigQuery: {JsonSerializer.Serialize(ticketConfigQueryDto)}");

        // 取得登入的accessToken
        var accessTokenDto = await _mediator.Send(new GetAccessTokenQuery()
        {
            CountryCode = request.CountryCode,
            Mobile = request.Mobile,
            Password = request.Password
        }, cancellationToken);
        _logger.LogInformation($"GetAccessTokenQuery: {JsonSerializer.Serialize(accessTokenDto)}");

        // 是否需要重新產生驗證碼
        var isRegenerateCaptcha = true;

        // 是否正在等待結果
        var isPending = false;

        GenerateCaptchaDto captchaDto = null;
        GetCaptchaAnswerDto captchaCode = null;
        while (true)
        {
            if (isRegenerateCaptcha)
            {
                _logger.LogInformation($"需要重新產生驗證碼");

                // 產生驗證碼
                captchaDto = await _mediator.Send(new GenerateCaptchaCommand
                {
                    SessionId = ticketConfigQueryDto.Result.Product.First().SessionId,
                    Token = accessTokenDto.UserInfo.Access_token
                }, cancellationToken);
                _logger.LogInformation($"GenerateCaptchaCommand");

                // 計算出驗證碼
                captchaCode = await _mediator.Send(new GetCaptchaAnswerQuery
                {
                    Data = captchaDto.Data
                }, cancellationToken);
                _logger.LogInformation($"GetCaptchaAnswerQuery: {JsonSerializer.Serialize(captchaCode)}");
            }

            // 預約票券
            var reserveResultDto = await _mediator.Send(new CreateReserveCommand
            {
                Products = new List<ReserveProduct>
                {
                    new ReserveProduct
                    {
                        // 選擇哪種票要改這邊
                        ProductId = ticketConfigQueryDto.Result.Product.First().Id,
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
            _logger.LogInformation($"CreateReserveCommand: {JsonSerializer.Serialize(reserveResultDto)}");

            // 如果結果是驗證碼錯誤就要重產驗證碼
            if (reserveResultDto.ErrCode.Equals(((int)ErrorCodeEnum.CaptchaFailed).ToString()))
            {
                _logger.LogInformation($"驗證碼錯誤，重新產生驗證碼");
                isRegenerateCaptcha = true;
                isPending = false;
                continue;
            }

            // 如果是Pending就定期重打API取得結果
            if (reserveResultDto.ErrCode.Equals(((int)ErrorCodeEnum.Pending).ToString()))
            {
                _logger.LogInformation($"等待結果中");
                isRegenerateCaptcha = false;
                isPending = true;
                await Task.Delay(1000, cancellationToken);
                continue;
            }

            // 如果是成功就回傳成功
            if (reserveResultDto.ErrCode.Equals("00") && reserveResultDto.Products.First().Status.Equals("RESERVED"))
            {
                _logger.LogInformation($"預約成功");
                return new AutoReserveDto { CreateReserveDto = reserveResultDto };
            }

            // 其他不知名的狀況(沒訂到之類的) 就繼續跑 ㄏㄏ
            _logger.LogInformation($"重跑");
            await Task.Delay(500, cancellationToken);
        }
    }
}