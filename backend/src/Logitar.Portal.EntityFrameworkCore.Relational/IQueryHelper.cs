using Logitar.Data;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public interface IQueryHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns);

  IQueryBuilder QueryFrom(TableId table);
}
