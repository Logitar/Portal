namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;

internal static class QueryableExtensions
{
  public static IQueryable<T> Page<T>(this IQueryable<T> query, int? skip, int? limit)
  {
    if (skip > 0)
    {
      query = query.Skip(skip.Value);
    }

    if (limit > 0)
    {
      query = query.Take(limit.Value);
    }

    return query;
  }
}
