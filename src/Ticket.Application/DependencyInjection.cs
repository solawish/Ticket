using ddddocrsharp;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ticket.Application.Helpers;
using Ticket.Application.Options;

namespace Ticket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddFluentValidation(AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => !x.IsDynamic));

        services.AddHttpClient("TicketPlus", client =>
        {
        });

        services.Configure<TicketPlusOptions>(
            configuration.GetSection(nameof(TicketPlusOptions))
        );

        services.AddSingleton<dddddocr>(new dddddocr(det: false, old: true, show_ad: false));
        services.AddSingleton<IOCRHelper, OCRHelper>();
        services.AddSingleton<IMD5Helper, MD5Helper>();

        services.AddMemoryCache();

        return services;
    }
}