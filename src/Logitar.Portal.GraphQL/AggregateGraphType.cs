﻿using GraphQL.Types;
using Logitar.Portal.Contracts;
using Logitar.Portal.GraphQL.Actors;

namespace Logitar.Portal.GraphQL;

internal abstract class AggregateGraphType<T> : ObjectGraphType<T> where T : Aggregate
{
  protected AggregateGraphType()
  {
    Field(x => x.CreatedBy, type: typeof(NonNullGraphType<ActorGraphType>))
      .Description("The actor who created the resource.");
    Field(x => x.CreatedOn)
      .Description("The date and time when the resource was created.");
    Field(x => x.UpdatedBy, type: typeof(NonNullGraphType<ActorGraphType>))
      .Description("The actor who updated the resource lastly.");
    Field(x => x.UpdatedOn)
      .Description("The date and time when the resource was updated lastly.");
    Field(x => x.Version)
      .Description("The version of the resource.");
  }
}
