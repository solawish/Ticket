using FluentValidation;

namespace Ticket.Application.Queries.TicketPlus.GetCaptchaAnswer;

public class GetCaptchaAnswerQueryValidator : AbstractValidator<GetCaptchaAnswerQuery>
{
    public GetCaptchaAnswerQueryValidator()
    {
        RuleFor(x => x.Data)
            .NotEmpty();
    }
}