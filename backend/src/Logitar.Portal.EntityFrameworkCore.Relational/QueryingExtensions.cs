using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder builder, ColumnId column, IEnumerable<Guid> ids)
  {
    if (!ids.Any())
    {
      return builder;
    }

    string[] aggregateIds = ids.Distinct().Select(id => new AggregateId(id).Value).ToArray();

    return builder.Where(column, Operators.IsIn(aggregateIds));
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
}
