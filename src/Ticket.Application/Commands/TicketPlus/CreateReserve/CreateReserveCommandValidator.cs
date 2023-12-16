using FluentValidation;
using Ticket.Domain.Entities.TicketPlus;

namespace Ticket.Application.Commands.TicketPlus.CreateReserve;

public class CreateReserveCommandValidator : AbstractValidator<CreateReserveCommand>
{
    public CreateReserveCommandValidator()
    {
        RuleFor(x => x.Products)
            .NotEmpty();

        RuleFor(x => x.Products)
            .ForEach(x => x.SetValidator(new ProductValidator()));

        RuleFor(x => x.Captcha)
            .NotEmpty();

        RuleFor(x => x.Captcha.Key)
            .NotEmpty();

        RuleFor(x => x.Captcha.Ans)
            .NotEmpty();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}

public class ProductValidator : AbstractValidator<ReserveProduct>
{
    public ProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Count)
            .GreaterThan(0);
    }
}