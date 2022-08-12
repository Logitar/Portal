using AutoMapper;
using Logitar.Portal.Core;

namespace Logitar.Portal.Web
{
  internal static class ListModelExtensions
  {
    public static ListModel<TOut> To<TIn, TOut>(this ListModel<TIn> list, IMapper mapper)
    {
      ArgumentNullException.ThrowIfNull(list);
      ArgumentNullException.ThrowIfNull(mapper);

      return new ListModel<TOut>(mapper.Map<IEnumerable<TOut>>(list.Items), list.Total);
    }
  }
}
