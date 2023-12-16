namespace Ticket.Application.Common.TicketPlus;

/// <summary>
/// Represents the cache keys used in the TicketPlus application.
/// </summary>
public static class CacheKey
{
    /// <summary>
    /// Represents the cache key for S3ProductInfo.
    /// </summary>
    public const string S3ProductInfoCacheKey = "S3ProductInfo_{0}";

    /// <summary>
    /// Represents the cache key for ProductConfig.
    /// </summary>
    public const string ProductConfigCacheKey = "ProductConfig_{0}";

    /// <summary>
    /// Represents the cache key for AreaConfig.
    /// </summary>
    public const string AreaConfigCacheKey = "AreaConfig_{0}";

    /// <summary>
    /// Represents the cache key for User.
    /// </summary>
    public const string UserCacheKey = "User_{0}";
}