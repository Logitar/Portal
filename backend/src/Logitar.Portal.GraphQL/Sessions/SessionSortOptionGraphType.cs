using GraphQL.Types;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Sessions;

internal class SessionSortOptionGraphType : SortOptionInputGraphType<SessionSortOption>
{
  public SessionSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<SessionSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
