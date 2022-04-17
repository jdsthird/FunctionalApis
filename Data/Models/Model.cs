namespace Data.Models;

public interface IModel<T> where T : notnull
{
    Id<T> Id { get; }
}

public record Model<T>(Id<T> Id) : IModel<T> where T : notnull;