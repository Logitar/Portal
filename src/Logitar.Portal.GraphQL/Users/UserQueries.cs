using GraphQL;
using GraphQL.Types;
using Logitar.Cms.Schema.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.GraphQL.Users;

internal static class UserQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<UserGraphType>("user")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Retrieves a single user.")
      .Arguments(
        new QueryArgument<IdGraphType>() { Name = "id", Description = "The unique identifier of the user." },
        new QueryArgument<StringGraphType>() { Name = "realm", Description = "The unique identifier or unique name of the realm in which the user resides." },
        new QueryArgument<StringGraphType>() { Name = "uniqueName", Description = "The unique name of the user." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IUserService, object?>().ReadAsync(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("realm"),
        context.GetArgument<string?>("uniqueName"),
        context.GetArgument<string?>("identifierKey"),
        context.GetArgument<string?>("identifierValue"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<UserSearchResultsGraphType>>("users")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Searches a list of users.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchUsersPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IUserService, object?>().SearchAsync(
        context.GetArgument<SearchUsersPayload>("payload"),
        context.CancellationToken
      ));
  }
}
