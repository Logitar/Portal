namespace Portal.Core
{
  public class PagedList<T> : List<T>
  {
    public PagedList() : base()
    {
    }
    public PagedList(int capacity) : base(capacity)
    {
    }
    public PagedList(IEnumerable<T> collection, long? total = null) : base(collection)
    {
      Total = total ?? collection?.LongCount() ?? 0;
    }

    public long Total { get; }
  }
}
