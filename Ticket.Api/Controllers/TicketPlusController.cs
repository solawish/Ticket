using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ticket.Application.Commands.TicketPlus.AutoReserve;
using Ticket.Application.Commands.TicketPlus.CreateReserve;
using Ticket.Application.Commands.TicketPlus.GenerateCaptcha;
using Ticket.Application.Commands.TicketPlus.InitialActivityCache;
using Ticket.Application.Commands.TicketPlus.InitialUser;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;
using Ticket.Application.Queries.TicketPlus.GetAreaConfig;
using Ticket.Application.Queries.TicketPlus.GetCaptchaAnswer;
using Ticket.Application.Queries.TicketPlus.GetProductConfig;
using Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

namespace Ticket.Api.Controllers;

/// <summary>
/// TicketPlus Controller
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class TicketPlusController : ControllerBase
{
    private readonly ILogger<TicketPlusController> _logger;
    private readonly IMediator _mediator;

    public TicketPlusController(
        ILogger<TicketPlusController> logger,
        IMediator mediator
        )
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// 取得S3票券資訊
    /// </summary>
    /// <param name="getProductInfoQuery"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("S3ProductInfo")]
    [ProducesResponseType(typeof(GetS3ProductInfoDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] GetS3ProductInfoQuery getProductInfoQuery)
    {
        var result = await _mediator.Send(getProductInfoQuery);
        return Ok(result);
    }

    /// <summary>
    /// 取得票券的資訊
    /// </summary>
    /// <param name="getProductConfigQuery"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("ProductConfig")]
    [ProducesResponseType(typeof(GetProductConfigDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] GetProductConfigQuery getProductConfigQuery)
    {
        var result = await _mediator.Send(getProductConfigQuery);
        return Ok(result);
    }

    /// <summary>
    /// 取得票區的資訊
    /// </summary>
    /// <param name="getAreaConfigQuery"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("AreaConfig")]
    [ProducesResponseType(typeof(GetAreaConfigDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] GetAreaConfigQuery getAreaConfigQuery)
    {
        var result = await _mediator.Send(getAreaConfigQuery);
        return Ok(result);
    }

    /// <summary>
    /// 登入(取得AccessToken)
    /// </summary>
    /// <param name="getAccessTokenQuery"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("AccessToken")]
    [ProducesResponseType(typeof(GetAccessTokenDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] GetAccessTokenQuery getAccessTokenQuery)
    {
        var result = await _mediator.Send(getAccessTokenQuery);
        return Ok(result);
    }

    /// <summary>
    /// 初始化活動快取
    /// </summary>
    /// <param name="initialActivityCacheCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("Activity/Cache")]
    [ProducesResponseType(typeof(InitialActivityCacheDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post([FromQuery] InitialActivityCacheCommand initialActivityCacheCommand)
    {
        var result = await _mediator.Send(initialActivityCacheCommand);
        return Ok(result);
    }

    /// <summary>
    /// 初始化使用者
    /// </summary>
    /// <param name="initialUserCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("User/Cache")]
    [ProducesResponseType(typeof(InitialUserCacheDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] InitialUserCacheCommand initialUserCommand)
    {
        var result = await _mediator.Send(initialUserCommand);
        return Ok(result);
    }

    /// <summary>
    /// 產生驗證碼圖片
    /// </summary>
    /// <param name="generateCaptchaCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Captcha")]
    [ProducesResponseType(typeof(GenerateCaptchaDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post([FromBody] GenerateCaptchaCommand generateCaptchaCommand)
    {
        var result = await _mediator.Send(generateCaptchaCommand);
        return Ok(result);
    }

    /// <summary>
    /// 解析驗證碼
    /// </summary>
    /// <param name="getCaptchaAnswerQuery"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Captcha/Parsing")]
    [ProducesResponseType(typeof(GetCaptchaAnswerDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromBody] GetCaptchaAnswerQuery getCaptchaAnswerQuery)
    {
        var result = await _mediator.Send(getCaptchaAnswerQuery);
        return Ok(result);
    }

    /// <summary>
    /// 預約票券
    /// </summary>
    /// <param name="createReserveCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Reserve")]
    [ProducesResponseType(typeof(CreateReserveDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post([FromBody] CreateReserveCommand createReserveCommand)
    {
        var result = await _mediator.Send(createReserveCommand);
        return Ok(result);
    }

    /// <summary>
    /// 自動預約
    /// </summary>
    /// <param name="autoReserveCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("AutoReserve")]
    [ProducesResponseType(typeof(AutoReserveDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post([FromBody] AutoReserveCommand autoReserveCommand, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(autoReserveCommand, cancellationToken);
        return Ok(result);
    }
}