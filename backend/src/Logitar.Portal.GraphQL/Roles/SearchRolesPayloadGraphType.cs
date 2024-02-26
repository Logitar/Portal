using GraphQL.Types;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Roles;

internal class SearchRolesPayloadGraphType : SearchPayloadInputGraphType<SearchRolesPayload>
{
  public SearchRolesPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<RoleSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
