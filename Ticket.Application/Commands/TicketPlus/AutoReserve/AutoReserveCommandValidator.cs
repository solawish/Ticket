using FluentValidation;

namespace Ticket.Application.Commands.TicketPlus.AutoReserve;

public class AutoReserveCommandValidator : AbstractValidator<AutoReserveCommand>
{
    public AutoReserveCommandValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty()
            .WithMessage("ActivityId is required.");

        RuleFor(x => x.Mobile)
            .NotEmpty()
            .WithMessage("Mobile is required.");

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .WithMessage("CountryCode is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(x => x.Count)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Count is required.");
    }
}