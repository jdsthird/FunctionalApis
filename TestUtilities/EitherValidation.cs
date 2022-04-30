using System.Net;
using Data.Errors;
using LanguageExt;
using NUnit.Framework;

namespace TestUtilities;

public static class EitherValidation
{
    private static void CodeEquals(this StatusCodeError error, HttpStatusCode expectedCode) =>
        Assert.AreEqual(expectedCode, error.Code);

    public static void IsErrorWithCode<T>(this Either<StatusCodeError, T> either, HttpStatusCode expectedCode)
    {
        Assert.IsTrue(either.IsLeft);
        either.IfLeft(error => error.CodeEquals(expectedCode));
    }

    public static void IsValid<T>(this Either<StatusCodeError, T> either, Action<T> validation)
    {
        Assert.IsTrue(either.IsRight);
        either.IfRight(validation);
    }
}