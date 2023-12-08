using ddddocrsharp;
using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ticket.Application.Helpers;
using Ticket.Application.Options;
using Ticket.Application.Services;

namespace Ticket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.AddHttpClient("TicketPlus", client =>
        {
        });

        services.Configure<TicketPlusOptions>(
            configuration.GetSection(nameof(TicketPlusOptions))
        );

        // get runtime path
        var runtimeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $"{runtimeDirectory}/ocr-test-268205-902fdaf2a2f9.json");

        services.AddSingleton<ImageAnnotatorClient>(ImageAnnotatorClient.Create());
        services.AddScoped<GoogleVisionService>();

        services.AddSingleton<dddddocr>(new dddddocr(det: false, old: true, show_ad: false));
        services.AddSingleton<OrcHelpers>();

        return services;
    }
}