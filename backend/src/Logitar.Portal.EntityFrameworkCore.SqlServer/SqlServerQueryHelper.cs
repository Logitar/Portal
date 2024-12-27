using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Portal.EntityFrameworkCore.Relational;

namespace Logitar.Portal.EntityFrameworkCore.SqlServer;

internal class SqlServerQueryHelper : QueryHelper
{
  public override IQueryBuilder From(TableId table) => SqlServerQueryBuilder.From(table);
}
