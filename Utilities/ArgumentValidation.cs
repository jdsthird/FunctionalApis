using System.Runtime.CompilerServices;

namespace Utilities;

public static class ArgumentValidation
{
    public static T ThrowIfNull<T>(this T value, [CallerArgumentExpression("value")] string? argumentName = null) =>
        value ?? throw new ArgumentNullException(argumentName);
}