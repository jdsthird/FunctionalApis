using Data.Models;

namespace Packing;

public record Item(Id<long> Id, string Name) : Model<long>(Id);