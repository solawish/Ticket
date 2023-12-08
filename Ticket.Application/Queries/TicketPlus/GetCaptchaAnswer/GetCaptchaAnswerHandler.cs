using MediatR;
using Svg;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using Ticket.Application.Helpers;
using Ticket.Application.Services;

namespace Ticket.Application.Queries.TicketPlus.GetCaptchaAnswer;

/// <summary>
/// 解析驗證碼
/// </summary>
public class GetCaptchaAnswerHandler : IRequestHandler<GetCaptchaAnswerQuery, GetCaptchaAnswerDto>
{
    private readonly GoogleVisionService _googleVisionService;
    private readonly OrcHelpers _orcHelpers;

    public GetCaptchaAnswerHandler(
        GoogleVisionService googleVisionService,
        OrcHelpers orcHelpers)
    {
        _googleVisionService = googleVisionService;
        _orcHelpers = orcHelpers;
    }

    public async Task<GetCaptchaAnswerDto> Handle(GetCaptchaAnswerQuery request, CancellationToken cancellationToken)
    {
        var unescape = Regex.Unescape(request.Data);
        using var resultStream = new MemoryStream();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(unescape));
        var svgDocument = SvgDocument.Open<SvgDocument>(stream);
        var bitmap = new Bitmap(213, 75);
        Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.White);
        svgDocument.Draw(bitmap);
        bitmap.Save(resultStream, ImageFormat.Png);
        bitmap.Save("result.jpg", ImageFormat.Png);
        resultStream.Position = 0;

        //var result = await _googleVisionService.GetTextFromImageAsync(resultStream);

        // stream to byte array
        var bytes = new byte[resultStream.Length];
        resultStream.Read(bytes, 0, bytes.Length);
        resultStream.Seek(0, SeekOrigin.Begin);

        var result = _orcHelpers.GetTextFromImage(bytes);

        return new GetCaptchaAnswerDto
        {
            CaptchaAnswer = result
        };
    }
}