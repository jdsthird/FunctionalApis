using System.Collections.Immutable;

namespace Data.Repositories;

public interface IQuery<T>
{
    ImmutableList<T> Filter(IEnumerable<T> items);
}