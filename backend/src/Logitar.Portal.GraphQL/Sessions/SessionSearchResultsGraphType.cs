using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Sessions;

internal class SessionSearchResultsGraphType : SearchResultsGraphType<SessionGraphType, SessionModel>
{
  public SessionSearchResultsGraphType() : base("SessionSearchResults", "Represents the results of a session search.")
  {
  }
}
