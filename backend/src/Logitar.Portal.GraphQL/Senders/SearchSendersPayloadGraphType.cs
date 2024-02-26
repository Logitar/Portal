using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Senders;

internal class SearchSendersPayloadGraphType : SearchPayloadInputGraphType<SearchSendersPayload>
{
  public SearchSendersPayloadGraphType() : base()
  {
    Field(x => x.Provider, type: typeof(SenderProviderGraphType))
      .Description("When specified, will filter out senders that do not have the specified type.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<SenderSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
