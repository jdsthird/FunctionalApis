using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Data.Models;
using Data.Repositories;
using NUnit.Framework;

namespace DataTests.Repositories;

public class DictionaryRepositoryTests
{
    private const string JaneDoe = "Jane Doe";
    private const string JaneSmith = "Jane Smith";
    private const string JohnDoe = "John Doe";
    private const string JohnSmith = "John Smith";
    
    private static readonly Func<Guid> IdGenerator = Guid.NewGuid;

    private static TestModel Model(string name) => new(Id<Guid>.TemporaryId(IdGenerator()), name);
    
    private static (TestRepo repo, ImmutableList<TestModel> models) Repo(params string[] names)
    {
        var repo = new TestRepo(IdGenerator);
        return (repo, names.Map(name => repo.Create(Model(name))).ToImmutableList());
    }

    [Test]
    public void Create_ThrowsOnNullInput()
    {
        var (repo, _) = Repo();
        Assert.Throws<NullReferenceException>(() =>
            repo.Create(null!));
    }
    
    [Test]
    public void Create_SetsAPermanentId()
    {
        var (repo, _) = Repo();
        var temporary = new TestModel(Id<Guid>.TemporaryId(IdGenerator()), JaneDoe);
        var (permanentId, permanentName) = repo.Create(temporary);
        Assert.AreEqual(JaneDoe, permanentName);
        Assert.IsFalse(permanentId.IsTemporary);
        Assert.AreNotEqual(temporary.Id, permanentId);
    }
    
    [Test]
    public void Create_AddsModelToItemsDictionary()
    {
        var (repo, _) = Repo();
        var temporary = Model(JaneDoe);
        var permanent = repo.Create(temporary);
        var read = repo.Read(permanent.Id);
        Assert.AreEqual(permanent, read);
    }

    [Test]
    public void Read_ThrowsIfIdIsNull()
    {
        var (repo, _) = Repo();
        Assert.Throws<ArgumentNullException>(() =>
            repo.Read(null!));
    }

    [Test]
    public void Read_ThrowsIfIdMissing()
    {
        var (repo, _) = Repo();
        Assert.Throws<KeyNotFoundException>(() =>
            repo.Read(Id<Guid>.TemporaryId(IdGenerator())));
    }

    [Test]
    public void Read_ReturnsMatchingModel()
    {
        var (repo, models) = Repo(JaneDoe);
        var model = models.Single();
        Assert.AreEqual(model, repo.Read(model.Id));
    }

    [Test]
    public void ReadAll_ThrowsIfQueryIsNull()
    {
        var (repo, _) = Repo();
        Assert.Throws<NullReferenceException>(() =>
            repo.ReadAll(null!));
    }

    [Test]
    public void ReadAll_ReturnsCorrectModels()
    {
        var (repo, _) = Repo(JaneDoe, JaneSmith, JohnDoe, JohnSmith);
        CollectionAssert.AreEquivalent(
            new[]{JaneDoe, JaneSmith},
            repo.ReadAll(new TestQuery()).Map(model => model.Name));
    }

    [Test]
    public void Destroy_ThrowsWhenModelIsNull()
    {
        var (repo, _) = Repo();
        Assert.Throws<NullReferenceException>(() =>
            repo.Destroy(null!));
    }

    [Test]
    public void Destroy_RemovesModelFromRepo()
    {
        var (repo, models) = Repo(JaneDoe);
        var model = models.Single();
        repo.Destroy(model);
        Assert.Throws<KeyNotFoundException>(() =>
            repo.Read(model.Id));
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