namespace Ticket.Application.Helpers;

public interface IMD5Helper
{
    /// <summary>
    /// Get MD5 Hash
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    string GetMd5Hash(string input);
}