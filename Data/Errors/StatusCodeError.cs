using System.Net;
using System.Text;
using LanguageExt;

namespace Data.Errors;

/// <summary>
/// Class to be used for capturing errors where they occur and propagating them safely
/// up the call stack as the left track of an Either. This allows for better separation
/// of concerns: the error can be identified as close to the source as possible, but
/// handled differently by each caller as appropriate.
/// </summary>
/// <param name="Code">
/// An HttpStatusCode signaling what went wrong. Will be 500 if the error was unexpected.
/// </param>
/// <param name="Message">
/// A user friendly message that can be displayed/returned to the public. Must NOT
/// contain any privileged or sensitive data!
/// </param>
/// <param name="Exception">
/// The caught exception that precipitated this Error, if one exists.
/// </param>
public record StatusCodeError(
    HttpStatusCode Code,
    string Message,
    Option<Exception> Exception = default)
{
    public StatusCodeError(Exception exception)
        : this(HttpStatusCode.InternalServerError,
            "Encountered an unexpected error",
            exception)
    {}
}

public static class StatusCodeErrorFunctions
{
    /// <summary>
    /// Formats the Error for internal logging. DO NOT expose this to clients as
    /// the output may contain diagnostic or other sensitive data.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static string Log(this StatusCodeError error) =>
        error.Exception.Match(
            exception => $"An exception was thrown: {exception.Log()}",
            () => $"An error occurred with code {error.Code} and message {error.Message}");

    private static string Log(this Exception exception)
    {
        var innerException = exception;
        var message = new StringBuilder();
        do
        {
            message.Append(exception.LogMessage());
            message.Insert(message.Length, Environment.NewLine, 2);
            innerException = innerException.InnerException;
        } while (innerException is not null);
        return message.ToString();
    }

    private static string LogMessage(this Exception exception) =>
        $"Message: {exception.Message}, Stack Trace: {exception.StackTrace}";
}