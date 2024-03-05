using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

internal class PostgresSearchHelper : SearchHelper
{
  protected override ConditionalOperator CreateOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}
