using System.Collections;
using LanguageExt;
using NUnit.Framework;

namespace TestUtilities;

public static class ValidationFunctions
{
    public static Try<T> IsEqual<T>(this T actual, T expected) => () =>
    {
        Assert.AreEqual(expected, actual);
        return actual;
    };

    public static Try<T> IsCollectionEqual<T>(this T actual, T expected) where T : ICollection => () =>
    {
        CollectionAssert.AreEqual(expected, actual);
        return actual;
    };

    public static Try<T> ValidateType<T>(this object thing) => () =>
    {
        Assert.IsInstanceOf<T>(thing);
        return (T) thing;
    };
}