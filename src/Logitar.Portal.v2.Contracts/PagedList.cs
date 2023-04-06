namespace Logitar.Portal.v2.Contracts;

public record PagedList<T>
{
  public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
  public long Total { get; init; }
}
