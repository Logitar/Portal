using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal class SearchSendersPayloadGraphType : SearchPayloadInputGraphType<SearchSendersPayload>
{
  public SearchSendersPayloadGraphType() : base()
  {
    Field(x => x.Realm, nullable: true)
      .Description("The unique identifier or unique name of the realm in which to search.");

    Field(x => x.Provider, type: typeof(ProviderTypeGraphType))
      .Description("When specified, will filter out senders that do not have the specified type.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<SenderSortOptionGraphType>>>))
      .DefaultValue(Enumerable.Empty<SenderSortOption>())
      .Description("The sort parameters of the search.");
  }
}
