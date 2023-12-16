using FluentValidation;

namespace Ticket.Application.Commands.TicketPlus.GenerateCaptcha;

public class GenerateCaptchaCommandValidator : AbstractValidator<GenerateCaptchaCommand>
{
    public GenerateCaptchaCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.SessionId)
            .NotEmpty();
    }
}