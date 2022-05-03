using System.Collections.Immutable;
using Data.Errors;
using Data.Models;
using LanguageExt;

namespace Data.Repositories;

public interface IRepository<TModel, TId, TQuery>
    where TModel : IModel<TId>
    where TId : notnull
    where TQuery : IQuery<TModel>
{
    EitherAsync<StatusCodeError, TModel> CreateAsync(TModel model);
    EitherAsync<StatusCodeError, TModel> ReadAsync(Id<TId> id);
    EitherAsync<StatusCodeError, ImmutableList<TModel>> ReadAllAsync(Option<TQuery> query = default);
    EitherAsync<StatusCodeError, TModel> UpdateAsync(TModel model);
    EitherAsync<StatusCodeError, Unit> DestroyAsync(Id<TId> id);
}