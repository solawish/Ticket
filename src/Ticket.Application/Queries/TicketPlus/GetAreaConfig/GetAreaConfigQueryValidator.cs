using FluentValidation;

namespace Ticket.Application.Queries.TicketPlus.GetAreaConfig;

public class GetAreaConfigQueryValidator : AbstractValidator<GetAreaConfigQuery>
{
    public GetAreaConfigQueryValidator()
    {
        RuleFor(x => x.TicketAreaId)
            .NotNull();
    }
}