using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestUtilities;

public static class ActionResultValidation
{
    public static T ValidateOkObjectResult<T>(this IActionResult result)
    {
        Assert.IsInstanceOf<OkObjectResult>(result);
        var objectResult = result as OkObjectResult;
        Assert.IsInstanceOf<T>(objectResult!.Value);
        return (T) objectResult.Value;
    }

    public static void ValidateOkResult(this IActionResult result) =>
        Assert.IsInstanceOf<OkResult>(result);

    public static void ValidateNoContentResult(this IActionResult result) =>
        Assert.IsInstanceOf<NoContentResult>(result);
}