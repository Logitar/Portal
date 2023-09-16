using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.GraphQL.Messages;

internal class MessageSearchResultsGraphType : SearchResultsGraphType<MessageGraphType, Message>
{
  public MessageSearchResultsGraphType() : base("MessageSearchResults", "Represents the results of a message search.")
  {
  }
}
