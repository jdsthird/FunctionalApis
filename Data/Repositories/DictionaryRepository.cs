using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using Data.Errors;
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

    protected DictionaryRepository(Func<TId> idGenerator)
    {
        _idGenerator = idGenerator.ThrowIfNull();
    }

    public EitherAsync<StatusCodeError, TModel> CreateAsync(TModel model)
    {
        if (!model.Id.IsTemporary)
            return new StatusCodeError(HttpStatusCode.BadRequest, "Model already has a permanent id.");

        var id = Id<TId>.PermanentId(_idGenerator());
        return _items[id] = model with {Id = id};
    }

    public EitherAsync<StatusCodeError, TModel> ReadAsync(Id<TId> id) =>
        _items.Get(id)
            .ToEitherAsync(() => new StatusCodeError(HttpStatusCode.NotFound, $"Object not found with id {id}"));

    public EitherAsync<StatusCodeError, ImmutableList<TModel>> ReadAllAsync(Option<TQuery> query = default) =>
        query.Match(
            q => q.Filter(_items.Values),
            () => _items.Values.ToImmutableList());

    public EitherAsync<StatusCodeError, TModel> UpdateAsync(TModel model)
    {
        if (model.Id.IsTemporary)
            return new StatusCodeError(HttpStatusCode.BadRequest, "Model has not yet been created");

        if (!_items.ContainsKey(model.Id))
            return new StatusCodeError(HttpStatusCode.BadRequest, "Model not in repository");

        return _items[model.Id] = model;
    }

    public EitherAsync<StatusCodeError, Unit> DestroyAsync(Id<TId> id)
    {
        _items.Remove(id);
        return Unit.Default;
    }
}