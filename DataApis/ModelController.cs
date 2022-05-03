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

    protected async Task<IActionResult> GetAsync(TId id) =>
        await _repo.ReadAsync(Id<TId>.PermanentId(id)).ReturnAsync();

    protected async Task<IActionResult> GetAllAsync(TQuery query) => await _repo.ReadAllAsync(query).ReturnAsync();

    protected async Task<IActionResult> PostAsync(TModel model) => await _repo.CreateAsync(model).ReturnAsync();

    protected async Task<IActionResult> PutAsync(TModel model) => await _repo.UpdateAsync(model).ReturnAsync();

    protected async Task<IActionResult> DeleteAsync(TId id) =>
        await _repo.DestroyAsync(Id<TId>.PermanentId(id)).ReturnAsync();
}