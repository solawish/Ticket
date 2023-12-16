using ddddocrsharp;

namespace Ticket.Application.Helpers;

public class OCRHelper : IOCRHelper
{
    private readonly dddddocr _oCRCore;

    public OCRHelper(dddddocr oCRCore)
    {
        this._oCRCore = oCRCore;
    }

    /// <summary>
    /// 取得辨識後的文字
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string GetTextFromImage(byte[] text)
    {
        var result = _oCRCore.classification(text);
        return result;
    }
}