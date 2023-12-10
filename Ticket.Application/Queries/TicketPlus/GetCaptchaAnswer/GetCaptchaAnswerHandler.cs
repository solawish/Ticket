using MediatR;
using Svg;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using Ticket.Application.Helpers;

namespace Ticket.Application.Queries.TicketPlus.GetCaptchaAnswer;

/// <summary>
/// 解析驗證碼
/// </summary>
public class GetCaptchaAnswerHandler : IRequestHandler<GetCaptchaAnswerQuery, GetCaptchaAnswerDto>
{
    private readonly IOCRHelper _oCRHelper;

    public GetCaptchaAnswerHandler(
        IOCRHelper oCRHelper)
    {
        _oCRHelper = oCRHelper;
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
        //bitmap.Save("result.jpg", ImageFormat.Png);
        resultStream.Position = 0;

        // stream to byte array
        var bytes = new byte[resultStream.Length];
        resultStream.Read(bytes, 0, bytes.Length);
        resultStream.Seek(0, SeekOrigin.Begin);

        var result = _oCRHelper.GetTextFromImage(bytes);

        return new GetCaptchaAnswerDto
        {
            CaptchaAnswer = result
        };
    }
}