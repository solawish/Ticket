namespace Ticket.Application.Common.TicketPlus;

/// <summary>
/// Represents the base response for TicketPlus.
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string ErrCode { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string ErrMsg { get; set; }

    /// <summary>
    /// Gets or sets the error detail.
    /// </summary>
    public string ErrDetail { get; set; }
}