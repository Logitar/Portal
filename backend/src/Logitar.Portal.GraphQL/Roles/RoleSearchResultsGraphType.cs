using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Roles;

internal class RoleSearchResultsGraphType : SearchResultsGraphType<RoleGraphType, RoleModel>
{
  public RoleSearchResultsGraphType() : base("RoleSearchResults", "Represents the results of a role search.")
  {
  }
}
