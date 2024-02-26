using Logitar.Portal.Contracts.Users;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Users;

internal class UserSearchResultsGraphType : SearchResultsGraphType<UserGraphType, User>
{
  public UserSearchResultsGraphType() : base("UserSearchResults", "Represents the results of an user search.")
  {
  }
}
