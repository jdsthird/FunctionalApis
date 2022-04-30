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
    Either<StatusCodeError, TModel> Create(TModel model);
    Either<StatusCodeError, TModel> Read(Id<TId> id);
    Either<StatusCodeError, ImmutableList<TModel>> ReadAll(Option<TQuery> query = default);
    Either<StatusCodeError, TModel> Update(TModel model);
    Either<StatusCodeError, Unit> Destroy(Id<TId> id);
}