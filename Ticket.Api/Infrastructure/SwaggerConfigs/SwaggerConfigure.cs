using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ticket.Api.Infrastructure.SwaggerConfigs;

public static class SwaggerConfigure
{
    public static void AddSwagger(this IServiceCollection services)
    {
        // Swagger Register
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerDefaultValues>();
            options.DescribeAllParametersInCamelCase();
        });

        // Add Fluent Validation Rules to Swagger
        services.AddFluentValidationRulesToSwagger();

        // HttpContextValidatorRegistry requires access to HttpContext
        services.AddHttpContextAccessor();
        // Register FV validators
        services.AddValidatorsFromAssemblyContaining<Program>(lifetime: ServiceLifetime.Scoped);
        // Add FV to Asp.net
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
    }
}