using FluentValidation;

namespace Ticket.Application.Commands.TicketPlus.AutoReserve;

public class CreateAutoReserveCommandValidator : AbstractValidator<CreateAutoReserveCommand>
{
    public CreateAutoReserveCommandValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();

        RuleFor(x => x.Mobile)
            .NotEmpty();

        RuleFor(x => x.CountryCode)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.Count)
            .NotEmpty()
            .GreaterThan(0);
    }
}