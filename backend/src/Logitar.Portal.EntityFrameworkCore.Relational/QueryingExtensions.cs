using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder builder, ColumnId column, IEnumerable<Guid> ids)
  {
    if (!ids.Any())
    {
      return builder;
    }

    string[] streamIds = ids.Distinct().Select(id => new StreamId(id).Value).ToArray();

    return builder.Where(column, Operators.IsIn(streamIds));
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    return query.ApplyPaging(payload.Skip, payload.Limit);
  }
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int skip, int limit)
  {
    if (skip > 0)
    {
      query = query.Skip(skip);
    }
    if (limit > 0)
    {
      query = query.Take(limit);
    }

    return query;
  }

  public static IQueryBuilder ApplyRealmFilter(this IQueryBuilder builder, ColumnId column, RealmModel? realm)
  {
    if (realm == null)
    {
      return builder.Where(column, Operators.IsNull());
    }

    TenantId tenantId = realm.GetTenantId();
    return builder.Where(column, Operators.IsEqualTo(tenantId.Value));
  }

  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQueryBuilder query) where T : class
  {
    return entities.FromQuery(query.Build());
  }
  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQuery query) where T : class
  {
    return entities.FromSqlRaw(query.Text, query.Parameters.ToArray());
  }
}
