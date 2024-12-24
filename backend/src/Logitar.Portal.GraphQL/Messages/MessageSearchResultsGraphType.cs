using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Messages;

internal class MessageSearchResultsGraphType : SearchResultsGraphType<MessageGraphType, MessageModel>
{
  public MessageSearchResultsGraphType() : base("MessageSearchResults", "Represents the results of a message search.")
  {
  }
}
