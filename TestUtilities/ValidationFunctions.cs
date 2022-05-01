using System.Collections;
using LanguageExt;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace TestUtilities;

public static class ValidationFunctions
{
    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
#pragma warning disable CA1806
    /// <summary>
    /// An adaptor function to allow functional style in unit tests that
    /// expect a void method. 
    /// </summary>
    /// <param name="assertion"></param>
    /// <typeparam name="T"></typeparam>
    public static void Assert<T>(this Try<T> assertion) => assertion.IfFailThrow();
#pragma warning restore CA1806
    
    public static Try<T> IsEqual<T>(this T actual, T expected) => () =>
    {
        AreEqual(expected, actual);
        return actual;
    };

    public static Try<T> IsCollectionEqual<T>(this T actual, T expected) where T : ICollection => () =>
    {
        CollectionAssert.AreEqual(expected, actual);
        return actual;
    };

    public static Try<T> ValidateType<T>(this object thing) => () =>
    {
        IsInstanceOf<T>(thing);
        return (T) thing;
    };
}