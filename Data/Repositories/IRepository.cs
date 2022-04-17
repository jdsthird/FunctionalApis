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
    Option<TModel> Read(Id<TId> id);
    ImmutableList<TModel> ReadAll(Option<TQuery> query = default);
    Either<StatusCodeError, TModel> Update(TModel model);
    Unit Destroy(TModel model);
}