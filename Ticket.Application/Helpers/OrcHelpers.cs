using ddddocrsharp;

namespace Ticket.Application.Helpers;

public class OrcHelpers
{
    private readonly dddddocr _oCRCore;

    public OrcHelpers(dddddocr oCRCore)
    {
        this._oCRCore = oCRCore;
    }

    public string GetTextFromImage(byte[] text)
    {
        var result = _oCRCore.classification(text);
        return result;
    }
}