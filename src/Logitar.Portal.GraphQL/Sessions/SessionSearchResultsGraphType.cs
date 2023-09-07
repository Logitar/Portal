using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.GraphQL.Sessions;

internal class SessionSearchResultsGraphType : SearchResultsGraphType<SessionGraphType, Session>
{
  public SessionSearchResultsGraphType() : base("SessionSearchResults", "Represents the results of a session search.")
  {
  }
}
