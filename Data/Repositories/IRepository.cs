using System.Collections.Immutable;
using Data.Models;
using LanguageExt;

namespace Data.Repositories;

public interface IRepository<TModel, TId, in TQuery>
    where TModel : IModel<TId>
    where TId : notnull
    where TQuery : IQuery<TModel>
{
    TModel Create(TModel model);
    TModel Read(Id<TId> id);
    ImmutableList<TModel> ReadAll(TQuery query);
    // TODO: Update
    Unit Destroy(TModel model);
}