namespace Logitar.Portal.Contracts
{
  public record ListModel<T>
  {
    public ListModel() : this(Enumerable.Empty<T>())
    {
    }
    public ListModel(IEnumerable<T> items) : this(items, items.LongCount())
    {
    }
    public ListModel(long total) : this(Enumerable.Empty<T>(), total)
    {
    }
    public ListModel(IEnumerable<T> items, long total)
    {
      Items = items;
      Total = total;
    }

    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public long Total { get; init; }
  }
}
