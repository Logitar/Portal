using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.GraphQL.Realms;

internal class SearchRealmsPayloadInputGraphType : SearchPayloadInputGraphType<SearchRealmsPayload>
{
  public SearchRealmsPayloadInputGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<RealmSortOptionInputGraphType>>>))
      .DefaultValue(Enumerable.Empty<RealmSortOption>())
      .Description("The sort parameters of the search.");
  }
}
