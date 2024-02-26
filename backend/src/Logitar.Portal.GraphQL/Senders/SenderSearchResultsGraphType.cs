using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Senders;

internal class SenderSearchResultsGraphType : SearchResultsGraphType<SenderGraphType, Sender>
{
  public SenderSearchResultsGraphType() : base("SenderSearchResults", "Represents the results of a sender search.")
  {
  }
}
