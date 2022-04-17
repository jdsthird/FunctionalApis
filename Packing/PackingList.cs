using System.Collections.Immutable;
using Data.Models;

namespace Packing;

public record PackingList(Id<long> Id, ImmutableList<Item> Items) : Model<long>(Id)
{
    public static PackingList Permanent(long id, ImmutableList<Item> items) => new(Id<long>.PermanentId(id), items);

    public static PackingList Temporary(long id, ImmutableList<Item> items) => new(Id<long>.TemporaryId(id), items);
}

public static class PackingListFunctions
{
    public static PackingList AddItem(this PackingList list, Item newItem) =>
        list with {Items = list.Items.Add(newItem)};

    public static PackingList RemoveItem(this PackingList list, Item oldItem) =>
        list with {Items = list.Items.Remove(oldItem)};
}