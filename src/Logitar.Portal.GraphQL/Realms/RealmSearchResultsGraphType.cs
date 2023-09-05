using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.GraphQL.Realms;

internal class RealmSearchResultsGraphType : SearchResultsGraphType<RealmGraphType, Realm>
{
  public RealmSearchResultsGraphType() : base("RealmSearchResults", "Represents the results of a realm search.")
  {
  }
}
