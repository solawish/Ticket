using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ticket.Application.Commands.TicketPlus.CreateReserveCommand;
using Ticket.Application.Commands.TicketPlus.GenerateRecaptchCommand;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;
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
    /// 取得票券資訊
    /// </summary>
    /// <param name="getProductInfoQuery"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("ProductInfo")]
    [ProducesResponseType(typeof(GetProductInfoDto), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] GetProductInfoQuery getProductInfoQuery)
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
}