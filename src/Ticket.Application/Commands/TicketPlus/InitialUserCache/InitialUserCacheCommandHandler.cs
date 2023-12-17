using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Ticket.Application.Common.TicketPlus;
using Ticket.Application.Queries.TicketPlus.GetAccessToken;

namespace Ticket.Application.Commands.TicketPlus.InitialUser;

/// <summary>
/// 初始化使用者
/// </summary>
public class InitialUserCacheCommandHandler : IRequestHandler<InitialUserCacheCommand, InitialUserCacheDto>
{
    private readonly IMediator _mediator;
    private readonly IMemoryCache _memoryCache;

    public InitialUserCacheCommandHandler(
        IMediator mediator,
        IMemoryCache memoryCache
        )
    {
        _mediator = mediator;
        _memoryCache = memoryCache;
    }

    public async Task<InitialUserCacheDto> Handle(InitialUserCacheCommand request, CancellationToken cancellationToken)
    {
        var accessTokenDto = await _mediator.Send(new GetAccessTokenQuery()
        {
            CountryCode = request.CountryCode,
            Mobile = request.Mobile,
            Password = request.Password
        }, cancellationToken);

        if (accessTokenDto is null || accessTokenDto.UserInfo is null)
        {
            throw new ArgumentException("登入失敗");
        }

        using var cacheEntry = _memoryCache.CreateEntry(string.Format(CacheKey.UserCacheKey, request.Mobile));
        cacheEntry.Value = accessTokenDto;
        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

        return new InitialUserCacheDto();
    }
}