using GraphQL.Types;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.GraphQL.Roles;

internal class SearchRolesPayloadGraphType : SearchPayloadInputGraphType<SearchRolesPayload>
{
  public SearchRolesPayloadGraphType() : base()
  {
    Field(x => x.Realm, nullable: true)
      .Description("The unique identifier or unique name of the realm in which to search.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<RoleSortOptionGraphType>>>))
      .DefaultValue(Enumerable.Empty<RoleSortOption>())
      .Description("The sort parameters of the search.");
  }
}
