using Logitar.Data;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public interface ISqlHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns);
  IQueryBuilder QueryFrom(TableId table);
}
