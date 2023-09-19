using GraphQL;
using GraphQL.Types;
using Logitar.Cms.Schema.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.GraphQL.Roles;

internal static class RoleQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<RoleGraphType>("role")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Retrieves a single role.")
      .Arguments(
        new QueryArgument<IdGraphType>() { Name = "id", Description = "The unique identifier of the role." },
        new QueryArgument<StringGraphType>() { Name = "realm", Description = "The unique identifier or unique name of the realm in which the role resides." },
        new QueryArgument<StringGraphType>() { Name = "uniqueName", Description = "The unique name of the role." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IRoleService, object?>().ReadAsync(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("realm"),
        context.GetArgument<string?>("uniqueName"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<RoleSearchResultsGraphType>>("roles")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Searches a list of roles.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchRolesPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IRoleService, object?>().SearchAsync(
        context.GetArgument<SearchRolesPayload>("payload"),
        context.CancellationToken
      ));
  }
}
