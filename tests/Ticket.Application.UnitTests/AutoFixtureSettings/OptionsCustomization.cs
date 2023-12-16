using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Ticket.Application.Options;

namespace Ticket.Application.UnitTests.AutoFixtureSettings;
// namespace $ext_safeprojectname$.Application.UnitTests.AutoFixtureSettings;

public class OptionsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var configuration = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json", false, true)
                            .Build();

        var apiClientConfigure = configuration.GetSection(nameof(TicketPlusOptions)).Get<TicketPlusOptions>();

        var options = Mock.Of<IOptions<TicketPlusOptions>>(_ => _.Value == apiClientConfigure);
        fixture.Inject(options);

        var apiMonitor =
            Mock.Of<IOptionsMonitor<TicketPlusOptions>>(_ => _.CurrentValue == Microsoft.Extensions.Options.Options.Create(apiClientConfigure).Value);
        fixture.Inject(apiMonitor);
    }
}