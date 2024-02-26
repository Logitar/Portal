using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Messages;

internal class MessageSortOptionGraphType : SortOptionInputGraphType<MessageSortOption>
{
  public MessageSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<MessageSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
