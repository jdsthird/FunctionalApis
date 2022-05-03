using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Apis;
using Data.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Packing;
using Utilities;

namespace Api.Controllers;

public class ItemController : ModelController<Item, long, ItemQuery>
{
    public ItemController(IRepository<Item, long, ItemQuery> repo) : base(repo) {}

    [FunctionName("DeleteItem")]
    public async Task<IActionResult> DeleteItemAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "{id:long}")]
        HttpRequest req,
        long id,
        ILogger<ItemController> logger) => await DeleteAsync(id);

    [FunctionName("GetItem")]
    public async Task<IActionResult> GetItemAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{id:long")]
        HttpRequest req,
        long id,
        ILogger<ItemController> logger) => await GetAsync(id);

    [FunctionName("GetItems")]
    public async Task<IActionResult> GetItemsAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "")]
        HttpRequest req,
        ILogger<ItemController> logger) =>
        await GetAllAsync(ItemQuery.FromQueryParams(req.GetQueryParameterDictionary()));

    [FunctionName("PostItem")]
    public async Task<IActionResult> PostItemAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "")]
        Item item,
        ILogger<ItemController> logger) => await PostAsync(item);

    [FunctionName("PutItem")]
    public async Task<IActionResult> PutItemAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{id:long}")]
        Item item,
        ILogger<ItemController> logger) => await PutAsync(item);
}
public record ItemQuery(Option<string> Name) : IQuery<Item>
{
    private const string NameKey = "name";
    
    public ImmutableList<Item> Filter(IEnumerable<Item> items) =>
        items.Filter(item => item.Name == Name).ToImmutableList();

    public static ItemQuery FromQueryParams(IDictionary<string, string> queryParameters) =>
        new(queryParameters.Get(NameKey));
}