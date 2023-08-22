using Logitar.Data;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public interface IQueryHelper
{
  void ApplyTextSearch(IQueryBuilder query, TextSearch search, params ColumnId[] columns);
  IQueryBuilder From(TableId table);
}
