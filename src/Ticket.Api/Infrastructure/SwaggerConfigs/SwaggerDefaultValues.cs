using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ticket.Api.Infrastructure.SwaggerConfigs;

/// <summary>
/// SwaggerDefaultValues
/// </summary>
/// <seealso cref="IOperationFilter"/>
public class SwaggerDefaultValues : IOperationFilter
{
    /// <summary>
    /// Applies the specified operation.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="context">The context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.First(p =>
            {
                bool equals = string.Equals(p.Name, parameter.Name, StringComparison.CurrentCultureIgnoreCase);
                return equals;
            });

            parameter.Description ??= description.ModelMetadata?.Description;

            parameter.Required |= description.IsRequired;
        }

        var fileUploadMime = "multipart/form-data";
        if (operation.RequestBody == null ||
            !operation.RequestBody.Content.Any(x =>
                x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
        {
            return;
        }

        var fileParams = context.MethodInfo.
            GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile));

        operation.RequestBody.Content[fileUploadMime].Schema.Properties =
            fileParams?.ToDictionary(
                k => k.Name,
                v => new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                });
    }
}