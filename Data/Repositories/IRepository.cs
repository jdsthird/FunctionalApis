using System.Collections.Immutable;
using Data.Errors;
using Data.Models;
using LanguageExt;

namespace Data.Repositories;

public interface IRepository<TModel, TId, in TQuery>
    where TModel : IModel<TId>
    where TId : notnull
    where TQuery : IQuery<TModel>
{
    Either<StatusCodeError, TModel> Create(TModel model);
    Option<TModel> Read(Id<TId> id);
    ImmutableList<TModel> ReadAll(TQuery query);
    Either<StatusCodeError, TModel> Update(TModel model);
    Unit Destroy(TModel model);
}