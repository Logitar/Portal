using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Realms;

internal class RealmSortOptionGraphType : SortOptionInputGraphType<RealmSortOption>
{
  public RealmSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<RealmSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
