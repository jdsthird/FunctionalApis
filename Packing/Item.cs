using Data.Models;

namespace Packing;

public record Item(Id<long> Id, string Name) : Model<long>(Id)
{
    public static Item Temporary(long id, string name) => new(Id<long>.TemporaryId(id), name);

    public static Item Permanent(long id, string name) => new(Id<long>.PermanentId(id), name);
}