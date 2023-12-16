using FluentValidation;
using Ticket.Application.Commands.TicketPlus.InitialUser;

namespace Ticket.Application.Commands.TicketPlus.InitialUserCache;

public class InitialUserCacheCommandValidator : AbstractValidator<InitialUserCacheCommand>
{
    public InitialUserCacheCommandValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty();

        RuleFor(x => x.Mobile)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}