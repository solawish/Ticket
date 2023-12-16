using FluentValidation;

namespace Ticket.Application.Queries.TicketPlus.GetAccessToken;

public class GetAccessTokenQueryValidator : AbstractValidator<GetAccessTokenQuery>
{
    public GetAccessTokenQueryValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty();

        RuleFor(x => x.Mobile)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}