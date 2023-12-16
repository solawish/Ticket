using FluentValidation;

namespace Ticket.Application.Queries.TicketPlus.GetProductConfig;

public class GetProductConfigQueryValidator : AbstractValidator<GetProductConfigQuery>
{
    public GetProductConfigQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotNull();
    }
}