using Logitar.Portal.Contracts;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class QueryExtensions
{
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    if (payload.Skip > 1)
    {
      query = query.Skip(payload.Skip);
    }
    if (payload.Limit > 1)
    {
      query = query.Take(payload.Limit);
    }

    return query;
  }
}
