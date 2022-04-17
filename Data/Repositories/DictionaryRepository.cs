using System.Collections.Concurrent;
using System.Collections.Immutable;
using Data.Models;
using LanguageExt;
using Utilities;

namespace Data.Repositories;

public class DictionaryRepository<TModel, TId, TQuery> : IRepository<TModel, TId, TQuery>
    where TModel : Model<TId>
    where TId : notnull
    where TQuery : IQuery<TModel>
{
    private readonly Func<TId> _idGenerator;
    private readonly IDictionary<Id<TId>, TModel> _items = new ConcurrentDictionary<Id<TId>, TModel>();

    public DictionaryRepository(Func<TId> idGenerator)
    {
        _idGenerator = idGenerator.ThrowIfNull();
    }

    public TModel Create(TModel model)
    {
        if (!model.Id.IsTemporary)
            throw new InvalidOperationException("Model already has a permanent id.");

        var id = Id<TId>.PermanentId(_idGenerator());
        return _items[id] = model with {Id = id};
    }

    public Option<TModel> Read(Id<TId> id) => _items.Get(id);

    public ImmutableList<TModel> ReadAll(TQuery query) => query.Filter(_items.Values);

    public Unit Destroy(TModel model)
    {
        _items.Remove(model.Id);
        return Unit.Default;
    }
}