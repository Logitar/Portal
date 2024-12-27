using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

internal class PostgresQueryHelper : QueryHelper
{
  public override IQueryBuilder From(TableId table) => PostgresQueryBuilder.From(table);

  protected override ConditionalOperator CreateOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}
