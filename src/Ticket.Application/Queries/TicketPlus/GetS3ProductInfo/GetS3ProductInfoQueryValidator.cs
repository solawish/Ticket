using FluentValidation;

namespace Ticket.Application.Queries.TicketPlus.GetS3ProductInfo;

public class GetS3ProductInfoQueryValidator : AbstractValidator<GetS3ProductInfoQuery>
{
    public GetS3ProductInfoQueryValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty();
    }
}