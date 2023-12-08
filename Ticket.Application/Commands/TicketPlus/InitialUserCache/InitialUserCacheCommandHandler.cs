using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Ticket.Application.Common;
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

        _memoryCache.Set(string.Format(Const.UserCacheKey, request.Mobile), accessTokenDto);
        return new InitialUserCacheDto();
    }
}