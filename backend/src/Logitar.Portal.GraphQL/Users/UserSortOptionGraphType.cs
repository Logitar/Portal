using GraphQL.Types;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Users;

internal class UserSortOptionGraphType : SortOptionInputGraphType<UserSortOption>
{
  public UserSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<UserSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
