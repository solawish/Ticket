using FluentValidation;

namespace Ticket.Application.Commands.TicketPlus.InitialActivityCache;

public class InitialActivityCacheCommandValidator : AbstractValidator<InitialActivityCacheCommand>
{
    public InitialActivityCacheCommandValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();
    }
}