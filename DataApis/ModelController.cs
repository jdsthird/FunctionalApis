using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Utilities;

namespace Apis;

public class ModelController<TModel, TId, TQuery>
    where TModel : IModel<TId>
    where TId : notnull
    where TQuery : IQuery<TModel>
{
    private readonly IRepository<TModel, TId, TQuery> _repo;

    protected ModelController(IRepository<TModel, TId, TQuery> repo)
    {
        _repo = repo.ThrowIfNull();
    }

    protected IActionResult Get(TId id) => _repo.Read(Id<TId>.PermanentId(id)).Return();

    protected IActionResult GetAll(TQuery query) => _repo.ReadAll(query).Return();

    protected IActionResult Post(TModel model) => _repo.Create(model).Return();

    protected IActionResult Put(TModel model) => _repo.Update(model).Return();

    protected IActionResult Delete(TId id) => _repo.Destroy(Id<TId>.PermanentId(id)).Return();
}