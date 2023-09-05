using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.GraphQL.Realms;

internal class RealmSortOptionInputGraphType : SortOptionInputGraphType<RealmSortOption>
{
  public RealmSortOptionInputGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<RealmSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
