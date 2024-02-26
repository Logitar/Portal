using GraphQL.Types;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.ApiKeys;

internal class SearchApiKeysPayloadGraphType : SearchPayloadInputGraphType<SearchApiKeysPayload>
{
  public SearchApiKeysPayloadGraphType() : base()
  {
    Field(x => x.HasAuthenticated, nullable: true)
      .Description("When specified, will filter API keys that have or do not have been authenticated.");
    Field(x => x.Status, type: typeof(ApiKeyStatusGraphType))
      .Description("The filter to apply to the search.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<ApiKeySortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
