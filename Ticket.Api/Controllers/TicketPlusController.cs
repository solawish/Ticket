using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ticket.Application.Commands.TicketPlus.CreateReserveCommand;
using Ticket.Application.Commands.TicketPlus.GenerateCaptchaCommand;
using Ticket.Application.Queries.TicketPlus.GetAccessTokenQuery;
using Ticket.Application.Queries.TicketPlus.GetCaptchaAnswerQuery;
using Ticket.Application.Queries.TicketPlus.GetProductInfoQuery;

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
}