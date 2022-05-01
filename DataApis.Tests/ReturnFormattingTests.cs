using System;
using System.Collections.Immutable;
using Apis;
using LanguageExt;
using NUnit.Framework;
using TestUtilities;

namespace DataApis.Tests;

public class ReturnFormattingTests
{
    private static readonly int[] Integers = {1, 2, 3};

    [Test]
    public void Return_Data_ThrowsExceptionWhenDataIsNull() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ((object?) null).Return().ValidateOkObjectResult<object>());

    [Test]
    public void Return_Data_ReturnsANoContentResultForAnEmptyCollection() =>
        ImmutableList<int>.Empty.Return().ValidateNoContentResult();

    [Test]
    public void Return_Data_ReturnsAnOkObjectResultForANonemptyCollection()
    {
        var result = Integers.Return().ValidateOkObjectResult<int[]>();
        CollectionAssert.AreEqual(Integers, result);
    }

    [Test]
    public void Return_Data_ReturnsAnOkResultForUnit() =>
        Unit.Default.Return().ValidateOkResult();

    [Test]
    public void Return_Data_ReturnsAnOkObjectResultForABasicObject() =>
        Assert.AreEqual(3, 3.Return().ValidateOkObjectResult<int>());
}
