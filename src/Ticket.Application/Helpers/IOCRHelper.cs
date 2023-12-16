namespace Ticket.Application.Helpers;

/// <summary>
/// The OCR Helper
/// </summary>
public interface IOCRHelper
{
    /// <summary>
    /// 取得辨識後的文字
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string GetTextFromImage(byte[] text);
}