using GraphQL.Types;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.GraphQL.Users;

internal class SearchUsersPayloadGraphType : SearchPayloadInputGraphType<SearchUsersPayload>
{
  public SearchUsersPayloadGraphType() : base()
  {
    Field(x => x.Realm, nullable: true)
      .Description("The unique identifier or unique name of the realm in which to search.");

    Field(x => x.HasPassword, nullable: true)
      .Description("When specified, will filter users who have or do not have a password.");
    Field(x => x.IsConfirmed, nullable: true)
      .Description("When specified, will filter users who are confirmed or not.");
    Field(x => x.IsDisabled, nullable: true)
      .Description("When specified, will filter users who are disabled or not.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<UserSortOptionGraphType>>>))
      .DefaultValue(Enumerable.Empty<UserSortOption>())
      .Description("The sort parameters of the search.");
  }
}
