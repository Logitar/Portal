using Logitar.Data;
using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class QueryBuilderExtensions
{
  public static IQueryBuilder ApplyIdInFilter(this IQueryBuilder builder, ColumnId idColumn, IEnumerable<Guid> ids)
  {
    if (!ids.Any())
    {
      return builder;
    }

    IEnumerable<string> aggregateIds = ids.Distinct().Select(id => new AggregateId(id).Value);

    return builder.Where(idColumn, Operators.IsIn(aggregateIds.ToArray()));
  }
}
