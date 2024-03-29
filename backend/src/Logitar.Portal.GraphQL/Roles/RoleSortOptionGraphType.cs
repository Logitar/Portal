﻿using GraphQL.Types;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Roles;

internal class RoleSortOptionGraphType : SortOptionInputGraphType<RoleSortOption>
{
  public RoleSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<RoleSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
