using GraphQL.Types;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Users;

internal class SearchUsersPayloadGraphType : SearchPayloadInputGraphType<SearchUsersPayload>
{
  public SearchUsersPayloadGraphType() : base()
  {
    // TODO(fpion): HasAuthenticated
    Field(x => x.HasPassword, nullable: true)
      .Description("When specified, will filter users who have or do not have a password.");
    Field(x => x.IsDisabled, nullable: true)
      .Description("When specified, will filter users who are disabled or not.");
    Field(x => x.IsConfirmed, nullable: true)
      .Description("When specified, will filter users who are confirmed or not.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<UserSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
