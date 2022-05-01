using System;
using System.Collections.Immutable;
using System.Net;
using Apis;
using Data.Errors;
using LanguageExt;
using static LanguageExt.Prelude;
using NUnit.Framework;
using TestUtilities;

namespace DataApis.Tests;

public class ReturnFormattingTests
{
    private const string ErrorMessage = "error message";
    private const HttpStatusCode ErrorStatusCode = HttpStatusCode.Conflict;
    private const string ExceptionMessage = "exception message";
    private const string ExceptionResultMessage = "Encountered an unexpected error";

    private static readonly Exception Exception = new(ExceptionMessage);
    private static readonly StatusCodeError ExpectedError = new(ErrorStatusCode, ErrorMessage);
    private static readonly int[] Integers = {1, 2, 3};
    private static readonly StatusCodeError UnexpectedError = new(Exception);

    #region Return Either

    [Test]
    public void Return_Either_ReturnsObjectResultWithErrorDataWhenDataIsError() =>
        Left<StatusCodeError, int>(ExpectedError)
            .Return()
            .ValidateObjectResult<string>((int) ErrorStatusCode)
            .Bind(resultMessage => resultMessage.IsEqual(ErrorMessage))
            .Assert();

    [Test]
    public void Return_Either_ReturnsOnlyPublicDataWhenDataIsError() =>
        Left<StatusCodeError, int>(UnexpectedError)
            .Return()
            .ValidateObjectResult<string>(500)
            .Bind(message => message.IsEqual(ExceptionResultMessage))
            .Assert();

    [Test]
    public void Return_Either_ReturnsANoContentResultForAnEmptyCollection() =>
        Right<StatusCodeError, ImmutableList<int>>(ImmutableList<int>.Empty)
            .Return()
            .ValidateNoContentResult()
            .Assert();

    [Test]
    public void Return_Either_ReturnsAnOkObjectResultForANonemptyCollection() =>
        Right<StatusCodeError, int[]>(Integers)
            .Return()
            .ValidateOkObjectResult<int[]>()
            .Bind(collection => collection.IsCollectionEqual(Integers))
            .Assert();

    [Test]
    public void Return_Either_ReturnsAnOkResultForUnit() =>
        Right<StatusCodeError, Unit>(unit)
            .Return()
            .ValidateOkResult()
            .Assert();

    [Test]
    public void Return_Either_ReturnsAnOkObjectResultForABasicObject() =>
        Right<StatusCodeError, int>(3)
            .Return()
            .ValidateOkObjectResult<int>()
            .Bind(result => result.IsEqual(3))
            .Assert();

    #endregion

    #region Return Object

    [Test]
    public void Return_Data_ThrowsExceptionWhenDataIsNull() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ((object?) null).Return().ValidateOkObjectResult<object>());

    [Test]
    public void Return_Data_ReturnsANoContentResultForAnEmptyCollection() =>
        ImmutableList<int>.Empty
            .Return()
            .ValidateNoContentResult()
            .Assert();

    [Test]
    public void Return_Data_ReturnsAnOkObjectResultForANonemptyCollection() =>
        Integers.Return()
            .ValidateOkObjectResult<int[]>()
            .Bind(collection => collection.IsCollectionEqual(Integers))
            .Assert();

    [Test]
    public void Return_Data_ReturnsAnOkResultForUnit() =>
        unit.Return()
            .ValidateOkResult()
            .Assert();

    [Test]
    public void Return_Data_ReturnsAnOkObjectResultForABasicObject() =>
        3.Return()
            .ValidateOkObjectResult<int>()
            .Bind(result => result.IsEqual(3))
            .Assert();

    #endregion

    #region Return Option

    [Test]
    public void Return_Option_ReturnsNotFoundResultForNone() =>
        Option<int>.None
            .Return()
            .ValidateNotFoundResult()
            .Assert();

    [Test]
    public void Return_Option_ThrowsExceptionWhenDataIsNull() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ((object?) null).Return().ValidateOkObjectResult<object>());

    [Test]
    public void Return_Option_ReturnsANoContentResultForAnEmptyCollection() =>
        ImmutableList<int>.Empty
            .Return()
            .ValidateNoContentResult()
            .Assert();

    [Test]
    public void Return_Option_ReturnsAnOkObjectResultForANonemptyCollection() =>
        Integers.Return()
            .ValidateOkObjectResult<int[]>()
            .Bind(collection => collection.IsCollectionEqual(Integers))
            .Assert();

    [Test]
    public void Return_Option_ReturnsAnOkResultForUnit() =>
        unit.Return()
            .ValidateOkResult()
            .Assert();

    [Test]
    public void Return_Option_ReturnsAnOkObjectResultForABasicObject() =>
        3.Return()
            .ValidateOkObjectResult<int>()
            .Bind(result => result.IsEqual(3))
            .Assert();

    #endregion
}
