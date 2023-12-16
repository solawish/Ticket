using Microsoft.AspNetCore.Mvc;

namespace Ticket.Api.Infrastructure.ApiVersion;

public static class ApiVersionConfigure
{
    public static void AddApiVersion(this IServiceCollection services)
    {
        services.AddApiVersioning(o =>
        {
            o.ReportApiVersions = true;
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new Asp.Versioning.ApiVersion(2, 0);
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
    }
}