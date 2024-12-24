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
    if (payload.Skip > 0)
    {
      query = query.Skip(payload.Skip);
    }
    if (payload.Limit > 0)
    {
      query = query.Take(payload.Limit);
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

  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQueryBuilder builder) where T : class
  {
    return entities.FromQuery(builder.Build());
  }
  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQuery query) where T : class
  {
    return entities.FromSqlRaw(query.Text, query.Parameters.ToArray());
  }
}
