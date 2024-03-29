﻿using Logitar.Data;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public interface ISearchHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns);
}
