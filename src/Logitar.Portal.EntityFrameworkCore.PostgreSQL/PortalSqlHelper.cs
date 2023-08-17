using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.Contracts;
using Logitar.Portal.EntityFrameworkCore.Relational;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

internal class PortalSqlHelper : SqlHelper, IPortalSqlHelper
{
  public void ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns)
  {
    if (!columns.Any())
    {
      return;
    }

    int terms = search.Terms.Count();
    if (terms > 0)
    {
      List<Condition> conditions = new(capacity: terms);

      foreach (SearchTerm term in search.Terms)
      {
        if (!string.IsNullOrWhiteSpace(term.Value))
        {
          string pattern = term.Value.Trim();
          conditions.Add(columns.Length == 1
            ? new OperatorCondition(columns.Single(), PostgresOperators.IsLikeInsensitive(pattern))
            : new OrCondition(columns
              .Select(column => new OperatorCondition(column, PostgresOperators.IsLikeInsensitive(pattern)))
              .ToArray())
          );
        }
      }

      if (conditions.Any())
      {
        switch (search.Operator)
        {
          case QueryOperator.And:
            builder.WhereAnd(conditions.ToArray());
            break;
          case QueryOperator.Or:
            builder.WhereOr(conditions.ToArray());
            break;
        }
      }
    }
  }
}
