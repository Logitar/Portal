using GraphQL.Types;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.GraphQL.ApiKeys;

internal class SearchApiKeysPayloadGraphType : SearchPayloadInputGraphType<SearchApiKeysPayload>
{
  public SearchApiKeysPayloadGraphType() : base()
  {
    Field(x => x.Realm, nullable: true)
      .Description("The unique identifier or unique name of the realm in which to search.");

    Field(x => x.Status, type: typeof(ApiKeyStatusGraphType))
      .Description("The filter to apply to the search.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<ApiKeySortOptionGraphType>>>))
      .DefaultValue(Enumerable.Empty<ApiKeySortOption>())
      .Description("The sort parameters of the search.");
  }
}
