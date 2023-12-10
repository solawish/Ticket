using AutoFixture;
using AutoFixture.Xunit2;

namespace Ticket.Application.UnitTests.AutoFixtureSettings;

public class AutoTestingDataAttribute : AutoDataAttribute
{
    public AutoTestingDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture().Customize(new TestingCustomization());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                   .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // 目前版本尚未支援 DateOnly, 手動設定
            fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

            return fixture;
        })
    {
    }
}