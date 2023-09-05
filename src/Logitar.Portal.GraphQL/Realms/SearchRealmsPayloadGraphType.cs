using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.GraphQL.Realms;

internal class SearchRealmsPayloadGraphType : SearchPayloadInputGraphType<SearchRealmsPayload>
{
  public SearchRealmsPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<RealmSortOptionGraphType>>>))
      .DefaultValue(Enumerable.Empty<RealmSortOption>())
      .Description("The sort parameters of the search.");
  }
}
