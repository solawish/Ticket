using AutoFixture.Dsl;
using System.Linq.Expressions;

namespace Ticket.Application.UnitTests.AutoFixtureSettings;

public static class AutoFixtureExtension
{
    public static IPostprocessComposer<T> WithValues<T, TProperty>(
        this IPostprocessComposer<T> composer,
        Expression<Func<T, TProperty>> propertyPicker,
        params TProperty[] values)
    {
        var queue = new Queue<TProperty>(values);

        return composer.With(propertyPicker, () => queue.Dequeue());
    }
}