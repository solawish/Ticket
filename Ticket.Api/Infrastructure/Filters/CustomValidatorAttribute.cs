using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ticket.Api.Infrastructure.Filters;

public class CustomValidatorAttribute : ActionFilterAttribute
{
    private readonly Type _validatorType;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomValidatorAttribute"/> class.
    /// </summary>
    /// <param name="validatorType">Type of the validator.</param>
    public CustomValidatorAttribute(Type validatorType)
    {
        _validatorType = validatorType;
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <inheritdoc/>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var parameters = context.ActionArguments;
        //if (parameters.Count <= 0) await base.OnActionExecutionAsync(context, next);

        var parameter = parameters.FirstOrDefault();

        if (parameter.Value is null)
        {
            var failureOutputModel = new
            {
                Code = "InvalidParameter",
                Status = 400,
                Message = "參數類別為空",
            };

            context.Result = new BadRequestObjectResult(failureOutputModel);
        }
        else
        {
            if (Activator.CreateInstance(_validatorType) is IValidator validator)
            {
                var validationResult = await validator.ValidateAsync(new ValidationContext<object>(parameter.Value));

                if (validationResult.IsValid.Equals(false))
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

                    var failureOutputModel = new
                    {
                        Code = "InvalidParameter",
                        Status = 400,
                        Message = $"參數驗證失敗: {string.Join(", ", errorMessages)}",
                    };

                    context.Result = new BadRequestObjectResult(failureOutputModel);
                }
            }
        }

        await base.OnActionExecutionAsync(context, next);
    }
}