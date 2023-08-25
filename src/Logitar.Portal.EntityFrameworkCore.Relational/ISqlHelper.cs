using Logitar.Data;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public interface ISqlHelper
{
  IQueryBuilder QueryFrom(TableId table);
}
