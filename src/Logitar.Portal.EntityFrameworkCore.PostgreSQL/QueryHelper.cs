using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts;
using Logitar.Portal.EntityFrameworkCore.Relational;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

internal class QueryHelper : IQueryHelper
{
  private readonly ISqlHelper _sqlHelper;

  public QueryHelper(ISqlHelper sqlHelper)
  {
    _sqlHelper = sqlHelper;
  }

  public void ApplyTextSearch(IQueryBuilder query, TextSearch search, params ColumnId[] columns)
  {
    if (columns.Any())
    {
      int termCount = search.Terms.Count();
      if (termCount > 0)
      {
        List<Condition> conditions = new(capacity: termCount);

        foreach (SearchTerm term in search.Terms)
        {
          string pattern = term.Value.Trim();

          if (columns.Length == 1)
          {
            conditions.Add(IsLike(columns.Single(), pattern));
          }
          else
          {
            conditions.Add(new OrCondition(columns.Select(column => IsLike(column, pattern)).ToArray()));
          }
        }

        if (conditions.Any())
        {
          switch (search.Operator)
          {
            case ConditionOperator.And:
              query.WhereAnd(conditions.ToArray());
              break;
            case ConditionOperator.Or:
              query.WhereOr(conditions.ToArray());
              break;
          }
        }
      }
    }
  }
  private static OperatorCondition IsLike(ColumnId column, string pattern) => new(column, PostgresOperators.IsLikeInsensitive(pattern));

  public IQueryBuilder From(TableId table) => _sqlHelper.QueryFrom(table);
}
