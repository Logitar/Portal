using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.GraphQL.Messages;

internal class SearchMessagesPayloadGraphType : SearchPayloadInputGraphType<SearchMessagesPayload>
{
  public SearchMessagesPayloadGraphType() : base()
  {
    Field(x => x.Realm, nullable: true)
      .Description("The unique identifier or unique name of the realm in which to search.");

    Field(x => x.IsDemo, nullable: true)
      .Description("When specified, will filter messages which are demo messages or not.");
    Field(x => x.Status, type: typeof(MessageStatusGraphType))
      .Description("When specified, will filter out messages that do not have the specified status.");
    Field(x => x.Template, nullable: true)
      .Description("When specified, will filter out messages that have not been compiled using the specified template identifier or unique name.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<MessageSortOptionGraphType>>>))
      .DefaultValue(Enumerable.Empty<MessageSortOption>())
      .Description("The sort parameters of the search.");
  }
}
