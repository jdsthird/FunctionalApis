using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestUtilities;

public static class ActionResultValidation
{
    public static Try<T> ValidateObjectResult<T>(this IActionResult result, int statusCode) =>
        result.ValidateType<OkObjectResult>()
            .Do(okObjectResult => Assert.AreEqual(statusCode, okObjectResult.StatusCode))
            .Bind(okObjectResult => okObjectResult.Value.ValidateType<T>());

    public static Try<T> ValidateOkObjectResult<T>(this IActionResult result)
    {
        return result.ValidateType<OkObjectResult>()
            .Bind(okObjectResult => okObjectResult.Value.ValidateType<T>());
    }

    public static Try<OkResult> ValidateOkResult(this IActionResult result) => result.ValidateType<OkResult>();

    public static Try<NoContentResult> ValidateNoContentResult(this IActionResult result) =>
        result.ValidateType<NoContentResult>();

    public static Try<NotFoundResult> ValidateNotFoundResult(this IActionResult result) => result.ValidateType<NotFoundResult>();
}