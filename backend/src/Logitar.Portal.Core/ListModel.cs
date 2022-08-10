namespace Logitar.Portal.Core
{
  public class ListModel<T>
  {
    /// <summary>
    /// Empty constructor for deserialization
    /// </summary>
    public ListModel()
    {
      Items = Enumerable.Empty<T>();
    }
    public ListModel(IEnumerable<T> items, long total)
    {
      Items = items ?? throw new ArgumentNullException(nameof(items));
      Total = total;
    }

    public IEnumerable<T> Items { get; set; }
    public long Total { get; set; }
  }
}
