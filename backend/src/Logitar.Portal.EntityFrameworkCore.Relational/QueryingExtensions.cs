using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
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

  public static IQueryBuilder ApplyRealmFilter(this IQueryBuilder builder, ColumnId column, Realm? realm)
  {
    if (realm == null)
    {
      return builder.Where(column, Operators.IsNull());
    }

    TenantId tenantId = new(new AggregateId(realm.Id).Value);
    return builder.Where(column, Operators.IsEqualTo(tenantId.Value));
  }
}
