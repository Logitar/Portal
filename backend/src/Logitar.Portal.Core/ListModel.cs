using AutoMapper;

namespace Logitar.Portal.Core
{
  public class ListModel<T>
  {
    private ListModel(IEnumerable<T> items, long total)
    {
      Items = items ?? throw new ArgumentNullException(nameof(items));
      Total = total;
    }

    public IEnumerable<T> Items { get; }
    public long Total { get; }

    public static ListModel<T> From<TSource>(ListModel<TSource> list, IMapper mapper)
    {
      ArgumentNullException.ThrowIfNull(list);
      ArgumentNullException.ThrowIfNull(mapper);

      return new ListModel<T>(mapper.Map<IEnumerable<T>>(list.Items), list.Total);
    }
    public static ListModel<T> From<TSource>(PagedList<TSource> list, IMapper mapper)
    {
      ArgumentNullException.ThrowIfNull(list);
      ArgumentNullException.ThrowIfNull(mapper);

      return new ListModel<T>(mapper.Map<IEnumerable<T>>(list), list.Total);
    }
  }
}
