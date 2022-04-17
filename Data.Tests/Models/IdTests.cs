using Data.Models;
using NUnit.Framework;

namespace DataTests.Models;

public class IdTests
{
    [Test]
    public void TemporaryId_ReturnsTemporaryId()
    {
        var id = Id<int>.TemporaryId(1);
        Assert.IsTrue(id.IsTemporary);
        int idValue = id;
        Assert.AreEqual(1, idValue);
    }

    [Test]
    public void PermanentId_ReturnsPermanentId()
    {
        var id = Id<int>.PermanentId(2);
        Assert.IsFalse(id.IsTemporary);
        int idValue = id;
        Assert.AreEqual(2, idValue);
    }
}