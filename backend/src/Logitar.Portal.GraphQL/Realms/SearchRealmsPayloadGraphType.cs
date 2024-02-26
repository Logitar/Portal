using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Realms;

internal class SearchRealmsPayloadGraphType : SearchPayloadInputGraphType<SearchRealmsPayload>
{
  public SearchRealmsPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<RealmSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
