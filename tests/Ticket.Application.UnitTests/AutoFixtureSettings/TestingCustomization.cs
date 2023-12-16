using AutoFixture;
using AutoFixture.AutoMoq;

namespace Ticket.Application.UnitTests.AutoFixtureSettings;

public class TestingCustomization : CompositeCustomization
{
    public TestingCustomization()
        : base(
            new AutoMoqCustomization
            {
                ConfigureMembers = true
            },
            new OptionsCustomization()
            )
    {
    }
}