using Logitar.Data;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public interface IPortalSqlHelper : ISqlHelper
{
  void ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns);
}
