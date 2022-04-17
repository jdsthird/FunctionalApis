using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using Data.Models;
using Data.Repositories;
using LanguageExt;
using NUnit.Framework;

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

    private static (TestRepo repo, ImmutableList<TestModel> models) Repo(params string[] names)
    {
        var repo = new TestRepo(IdGenerator);
        return (repo, names.Map(name => repo.Create(Model(name)).RightAsEnumerable().First()).ToImmutableList());
    }

    [Test]
    public void Create_ThrowsOnNullInput()
    {
        var (repo, _) = Repo();
        Assert.Throws<NullReferenceException>(() =>
            repo.Create(null!));
    }

    [Test]
    public void Create_FailsIfModelIsPermanent()
    {
        var (repo, _) = Repo();
        var result = repo.Create(Model(JaneDoe, true));
        Assert.IsTrue(result.IsLeft);
        result.IfLeft(error => Assert.AreEqual(HttpStatusCode.BadRequest, error.Code));
    }
    
    [Test]
    public void Create_SetsAPermanentId()
    {
        var (repo, _) = Repo();
        var temporary = new TestModel(Id<Guid>.TemporaryId(IdGenerator()), JaneDoe);
        var (permanentId, permanentName) = repo.Create(temporary).RightAsEnumerable().First();
        Assert.AreEqual(JaneDoe, permanentName);
        Assert.IsFalse(permanentId.IsTemporary);
        Assert.AreNotEqual(temporary.Id, permanentId);
    }
    
    [Test]
    public void Create_AddsModelToItemsDictionary()
    {
        var (repo, _) = Repo();
        var temporary = Model(JaneDoe);
        var permanent = repo.Create(temporary).RightAsEnumerable().First();
        var read = repo.Read(permanent.Id);
        Assert.AreEqual(permanent, (TestModel)read);
    }

    [Test]
    public void Read_ThrowsIfIdIsNull()
    {
        var (repo, _) = Repo();
        Assert.Throws<ArgumentNullException>(() =>
            repo.Read(null!));
    }

    [Test]
    public void Read_ReturnsNoneIfIdMissing()
    {
        var (repo, _) = Repo();
        Assert.IsTrue(repo.Read(Id<Guid>.TemporaryId(IdGenerator())).IsNone);
    }

    [Test]
    public void Read_ReturnsMatchingModel()
    {
        var (repo, models) = Repo(JaneDoe);
        var model = models.Single();
        Assert.AreEqual(model, (TestModel)repo.Read(model.Id));
    }

    [Test]
    public void ReadAll_ReturnsAllModelsIfNoQueryPassed()
    {
        var (repo, models) = Repo(JaneDoe, JaneSmith, JohnDoe, JohnSmith);
        CollectionAssert.AreEquivalent(models, repo.ReadAll());
    }

    [Test]
    public void ReadAll_ReturnsCorrectModelsForQuery()
    {
        var (repo, _) = Repo(JaneDoe, JaneSmith, JohnDoe, JohnSmith);
        CollectionAssert.AreEquivalent(
            new[]{JaneDoe, JaneSmith},
            repo.ReadAll(new TestQuery()).Map(model => model.Name));
    }

    [Test]
    public void Update_ErrorsIfModelIsNull()
    {
        var (repo, _) = Repo();
        Assert.Throws<NullReferenceException>(() =>
            repo.Update(null!));
    }

    [Test]
    public void Update_ErrorsIfModelIsTemporary()
    {
        var (repo, _) = Repo();
        var result = repo.Update(Model(JaneDoe));
        Assert.IsTrue(result.IsLeft);
        result.IfLeft(error => Assert.AreEqual(HttpStatusCode.BadRequest, error.Code));
    }

    [Test]
    public void Update_ErrorsIfModelNotInRepository()
    {
        var (repo, _) = Repo();
        var result = repo.Update(Model(JaneDoe, true));
        Assert.IsTrue(result.IsLeft);
        result.IfLeft(error => Assert.AreEqual(HttpStatusCode.BadRequest, error.Code));
    }

    [Test]
    public void Update_ReturnsModelUnchangedWhenSuccessful()
    {
        var (repo, models) = Repo(JaneDoe);
        var updatedModel = models.Single() with {Name = JaneSmith};
        Assert.AreEqual(updatedModel, repo.Update(updatedModel));
    }

    [Test]
    public void Update_UpdatesModelInRepo()
    {
        var (repo, models) = Repo(JaneDoe);
        var updatedModel = models.Single() with {Name = JaneSmith};
        repo.Update(updatedModel);
        Assert.AreEqual(updatedModel, (TestModel)repo.Read(updatedModel.Id));
    }

    [Test]
    public void Destroy_ThrowsWhenModelIsNull()
    {
        var (repo, _) = Repo();
        Assert.Throws<NullReferenceException>(() =>
            repo.Destroy(null!));
    }

    [Test]
    public void Destroy_DoesNotErrorWhenModelNotInRepo()
    {
        var (repo, _) = Repo();
        var model = Model(JaneDoe);
        Assert.AreEqual(Unit.Default, repo.Destroy(model));
    }

    [Test]
    public void Destroy_RemovesModelFromRepo()
    {
        var (repo, models) = Repo(JaneDoe);
        var model = models.Single();
        repo.Destroy(model);
        Assert.IsTrue(repo.Read(model.Id).IsNone);
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