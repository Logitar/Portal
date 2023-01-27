namespace Logitar.Portal.Core.Models
{
  public class ListModel<T>
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

    public IEnumerable<T> Items { get; set; }
    public long Total { get; set; }
  }
}
