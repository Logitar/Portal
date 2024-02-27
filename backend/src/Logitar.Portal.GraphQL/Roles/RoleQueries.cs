using GraphQL;
using GraphQL.Types;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.GraphQL.Roles;

internal static class RoleQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<RoleGraphType>("role")
      .Authorize()
      .Description("Retrieves a single role.")
      .Arguments(
        new QueryArgument<IdGraphType>() { Name = "id", Description = "The unique identifier of the role." },
        new QueryArgument<StringGraphType>() { Name = "uniqueName", Description = "The unique name of the role." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IRoleService, object?>().ReadAsync(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("uniqueName"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<RoleSearchResultsGraphType>>("roles")
      .Authorize()
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
