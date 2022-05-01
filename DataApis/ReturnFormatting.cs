using System.Collections;
using Data.Errors;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace Apis;

public static class ReturnFormatting
{
    public static IActionResult Return<T>(this Either<StatusCodeError, T> either) =>
        either.Match(Return, Return);

    private static IActionResult Return(this StatusCodeError error) =>
        new ObjectResult(error.Message) {StatusCode = (int) error.Code};
    
    public static IActionResult Return<T>(this Option<T> option) =>
        option.Match(
            data => data.Return(),
            () => new NotFoundResult());

    public static IActionResult Return<T>(this T data) =>
        data switch
        {
            ICollection collection => collection.ReturnCollection(),
            Unit => new OkResult(),
            object obj => obj.ReturnObject(),
            _ => throw new ArgumentOutOfRangeException(nameof(data), "Cannot be null."),
        };

    private static IActionResult ReturnCollection<T>(this T data) where T : ICollection =>
        data.Count < 1
            ? new NoContentResult()
            : data.ReturnObject();

    private static IActionResult ReturnObject(this object obj) => new OkObjectResult(obj);
}