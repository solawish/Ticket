using System.Security.Cryptography;
using System.Text;

namespace Ticket.Application.Helpers;

public class MD5Helper : IMD5Helper
{
    /// <summary>
    /// Get MD5 Hash
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string GetMd5Hash(string input)
    {
        var result = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(input))).ToLower();
        return result;
    }
}