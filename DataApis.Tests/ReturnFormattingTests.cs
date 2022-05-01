using System;
using System.Collections.Immutable;
using Apis;
using LanguageExt;
using static LanguageExt.Prelude;
using NUnit.Framework;
using TestUtilities;

namespace DataApis.Tests;

public class ReturnFormattingTests
{
    private static readonly int[] Integers = {1, 2, 3};

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
            .IfFailThrow();

    [Test]
    public void Return_Data_ReturnsAnOkObjectResultForANonemptyCollection() =>
        Integers.Return()
            .ValidateOkObjectResult<int[]>()
            .Bind(collection => collection.IsCollectionEqual(Integers))
            .IfFailThrow();

    [Test]
    public void Return_Data_ReturnsAnOkResultForUnit() =>
        unit.Return()
            .ValidateOkResult()
            .IfFailThrow();

    [Test]
    public void Return_Data_ReturnsAnOkObjectResultForABasicObject() =>
        3.Return()
            .ValidateOkObjectResult<int>()
            .Bind(result => result.IsEqual(3))
            .IfFailThrow();

    #endregion

    #region Return Option

    [Test]
    public void Return_Option_ReturnsNotFoundResultForNone() =>
        Option<int>.None
            .Return()
            .ValidateNotFoundResult()
            .IfFailThrow();

    [Test]
    public void Return_Option_ThrowsExceptionWhenDataIsNull() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ((object?) null).Return().ValidateOkObjectResult<object>());

    [Test]
    public void Return_Option_ReturnsANoContentResultForAnEmptyCollection() =>
        ImmutableList<int>.Empty
            .Return()
            .ValidateNoContentResult()
            .IfFailThrow();

    [Test]
    public void Return_Option_ReturnsAnOkObjectResultForANonemptyCollection() =>
        Integers.Return()
            .ValidateOkObjectResult<int[]>()
            .Bind(collection => collection.IsCollectionEqual(Integers))
            .IfFailThrow();

    [Test]
    public void Return_Option_ReturnsAnOkResultForUnit() =>
        unit.Return()
            .ValidateOkResult()
            .IfFailThrow();

    [Test]
    public void Return_Option_ReturnsAnOkObjectResultForABasicObject() =>
        3.Return()
            .ValidateOkObjectResult<int>()
            .Bind(result => result.IsEqual(3))
            .IfFailThrow();

    #endregion
}
