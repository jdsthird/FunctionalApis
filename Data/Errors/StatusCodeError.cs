using System.Net;
using System.Text;
using LanguageExt;

namespace Data.Errors;

public record StatusCodeError(HttpStatusCode Code, string Message, Option<Exception> Exception = default)
{
    public StatusCodeError(Exception exception)
        : this(HttpStatusCode.InternalServerError,
            "Encountered an unexpected error",
            exception)
    {}
}

public static class StatusCodeErrorFunctions
{
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