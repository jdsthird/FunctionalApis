using System.Collections.Generic;
using System.Collections.Immutable;
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
    public IActionResult DeleteItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "{id:long}")]
        HttpRequest req,
        long id,
        ILogger<ItemController> logger) => Delete(id);

    [FunctionName("GetItem")]
    public IActionResult GetItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{id:long")]
        HttpRequest req,
        long id,
        ILogger<ItemController> logger) => Get(id);

    [FunctionName("GetItems")]
    public IActionResult GetItems(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "")]
        HttpRequest req,
        ILogger<ItemController> logger) =>
        GetAll(ItemQuery.FromQueryParams(req.GetQueryParameterDictionary()));

    [FunctionName("PostItem")]
    public IActionResult PostItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "")]
        Item item,
        ILogger<ItemController> logger) => Post(item);

    [FunctionName("PutItem")]
    public IActionResult PutItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{id:long}")]
        Item item,
        ILogger<ItemController> logger) => Put(item);
}
public record ItemQuery(Option<string> Name) : IQuery<Item>
{
    private const string NameKey = "name";
    
    public ImmutableList<Item> Filter(IEnumerable<Item> items) =>
        items.Filter(item => item.Name == Name).ToImmutableList();

    public static ItemQuery FromQueryParams(IDictionary<string, string> queryParameters) =>
        new(queryParameters.Get(NameKey));
}