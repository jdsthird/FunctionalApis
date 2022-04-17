using LanguageExt;

namespace Utilities;

public static class DictionaryHelpers
{
    public static Option<TValue> Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>
        dictionary.TryGetValue(key, out var result)
            ? result
            : Option<TValue>.None;
}