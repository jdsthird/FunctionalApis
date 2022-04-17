using LanguageExt;

namespace Data.Models;

public record Id<T> where T : notnull
{
    private Option<T> Permanent { get; }
    
    private Option<T> Temporary { get; }

    public bool IsTemporary { get; }

    private Id(Option<T> permanent, Option<T> temporary)
    {
        if (permanent.IsSome && temporary.IsSome)
            throw new ArgumentOutOfRangeException(null, "Id cannot be both permanent and temporary");

        IsTemporary = temporary.IsSome;
        Permanent = permanent;
        Temporary = temporary;
    }

    public static implicit operator T(Id<T> id) =>
        id.IsTemporary
            ? id.Temporary.Match(v => v, () => throw new InvalidOperationException())
            : id.Permanent.Match(v => v, () => throw new InvalidOperationException());

    public static Id<T> TemporaryId(T value) => new(Option<T>.None, value);

    public static Id<T> PermanentId(T value) => new(value, Option<T>.None);
}