using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Data.Models;
using Data.Repositories;
using LanguageExt;
using NUnit.Framework;
using TestUtilities;

namespace DataTests.Repositories;

public class DictionaryRepositoryTests
{
    private const string JaneDoe = "Jane Doe";
    private const string JaneSmith = "Jane Smith";
    private const string JohnDoe = "John Doe";
    private const string JohnSmith = "John Smith";
    
    private static readonly Func<Guid> IdGenerator = Guid.NewGuid;

    private static TestModel Model(string name, bool isPermanent = false)
    {
        var idValue = IdGenerator();
        var id = isPermanent
            ? Id<Guid>.PermanentId(idValue)
            : Id<Guid>.TemporaryId(idValue);
        return new TestModel(id, name);
    }

    private static async Task<(TestRepo repo, ImmutableList<TestModel> models)> BuildRepoAsync(params string[] names)
    {
        var repo = new TestRepo(IdGenerator);
        var (_, models) = await names.Map(name => repo.CreateAsync(Model(name)))
            .Partition();
        return (repo, models!.ToImmutableList());
    }

    [Test]
    public async Task Create_ThrowsOnNullInputAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        Assert.Throws<NullReferenceException>(() =>
            repo.CreateAsync(null!));
    }

    [Test]
    public async Task Create_FailsIfModelIsPermanentAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        var result = await repo.CreateAsync(Model(JaneDoe, true));
        Assert.IsTrue(result.IsLeft);
        result.IfLeft(error => Assert.AreEqual(HttpStatusCode.BadRequest, error.Code));
    }
    
    [Test]
    public async Task Create_SetsAPermanentIdAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        var temporary = new TestModel(Id<Guid>.TemporaryId(IdGenerator()), JaneDoe);
        var either = await repo.CreateAsync(temporary);
        Assert.IsTrue(either.IsRight);
        var (permanentId, permanentName) = either.RightAsEnumerable().First();
        Assert.AreEqual(JaneDoe, permanentName);
        Assert.IsFalse(permanentId.IsTemporary);
        Assert.AreNotEqual(temporary.Id, permanentId);
    }
    
    [Test]
    public async Task Create_AddsModelToItemsDictionaryAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        var temporary = Model(JaneDoe);
        var either = await repo.CreateAsync(temporary);
        Assert.IsTrue(either.IsRight);
        var permanent = either.RightAsEnumerable().First();
        var read = await repo.ReadAsync(permanent.Id);
        Assert.AreEqual(permanent, (TestModel)read);
    }

    [Test]
    public async Task Read_ThrowsIfIdIsNullAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        Assert.Throws<ArgumentNullException>(() =>
            repo.ReadAsync(null!));
    }

    [Test]
    public async Task Read_ReturnsNoneIfIdMissingAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        (await repo.ReadAsync(Id<Guid>.TemporaryId(IdGenerator()))).IsErrorWithCode(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Read_ReturnsMatchingModelAsync()
    {
        var (repo, models) = await BuildRepoAsync(JaneDoe);
        var model = models.Single();
        Assert.AreEqual(model, (TestModel) await repo.ReadAsync(model.Id));
    }

    [Test]
    public async Task ReadAll_ReturnsAllModelsIfNoQueryPassedAsync()
    {
        var (repo, expectedModels) = await BuildRepoAsync(JaneDoe, JaneSmith, JohnDoe, JohnSmith);
        (await repo.ReadAllAsync()).IsValid(models =>
        CollectionAssert.AreEquivalent(expectedModels, models));
    }

    [Test]
    public async Task ReadAll_ReturnsCorrectModelsForQueryAsync()
    {
        var (repo, _) = await BuildRepoAsync(JaneDoe, JaneSmith, JohnDoe, JohnSmith);
        var result = await repo.ReadAllAsync(new TestQuery());
        result.IsValid(models =>
            CollectionAssert.AreEquivalent(
                new[] {JaneDoe, JaneSmith},
                models.Map(model => model.Name)));
    }

    [Test]
    public async Task Update_ErrorsIfModelIsNullAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        Assert.Throws<NullReferenceException>(() =>
            repo.UpdateAsync(null!));
    }

    [Test]
    public async Task Update_ErrorsIfModelIsTemporaryAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        var result = await repo.UpdateAsync(Model(JaneDoe));
        Assert.IsTrue(result.IsLeft);
        result.IfLeft(error => Assert.AreEqual(HttpStatusCode.BadRequest, error.Code));
    }

    [Test]
    public async Task Update_ErrorsIfModelNotInRepositoryAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        var result = await repo.UpdateAsync(Model(JaneDoe, true));
        Assert.IsTrue(result.IsLeft);
        result.IfLeft(error => Assert.AreEqual(HttpStatusCode.BadRequest, error.Code));
    }

    [Test]
    public async Task Update_ReturnsModelUnchangedWhenSuccessfulAsync()
    {
        var (repo, models) = await BuildRepoAsync(JaneDoe);
        var updatedModel = models.Single() with {Name = JaneSmith};
        Assert.AreEqual(updatedModel, await repo.UpdateAsync(updatedModel));
    }

    [Test]
    public async Task Update_UpdatesModelInRepoAsync()
    {
        var (repo, models) = await BuildRepoAsync(JaneDoe);
        var updatedModel = models.Single() with {Name = JaneSmith};
        await repo.UpdateAsync(updatedModel);
        Assert.AreEqual(updatedModel, (TestModel) await repo.ReadAsync(updatedModel.Id));
    }

    [Test]
    public async Task Destroy_ThrowsWhenIdIsNullAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        Assert.Throws<ArgumentNullException>(() =>
            repo.DestroyAsync(null!));
    }

    [Test]
    public async Task Destroy_DoesNotErrorWhenModelNotInRepoAsync()
    {
        var (repo, _) = await BuildRepoAsync();
        var model = Model(JaneDoe);
        Assert.AreEqual(Unit.Default, await repo.DestroyAsync(model.Id));
    }

    [Test]
    public async Task Destroy_RemovesModelFromRepoAsync()
    {
        var (repo, models) = await BuildRepoAsync(JaneDoe);
        var (id, _) = models.Single();
        await repo.DestroyAsync(id);
        (await repo.ReadAsync(id)).IsErrorWithCode(HttpStatusCode.NotFound);
    }

    private record TestModel(Id<Guid> Id, string Name) : Model<Guid>(Id);

    private record TestQuery : IQuery<TestModel>
    {
        public ImmutableList<TestModel> Filter(IEnumerable<TestModel> models) =>
            models.Where(m => m.Name.StartsWith("Jane")).ToImmutableList();
    }

    private class TestRepo : DictionaryRepository<TestModel, Guid, TestQuery>
    {
        public TestRepo(Func<Guid> idGenerator) : base(idGenerator){}
    }
}