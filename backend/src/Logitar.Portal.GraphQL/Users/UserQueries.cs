using GraphQL;
using GraphQL.Types;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.GraphQL.Users;

internal static class UserQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<UserGraphType>("user")
      .Authorize()
      .Description("Retrieves a single user.")
      .Arguments(
        new QueryArgument<IdGraphType>() { Name = "id", Description = "The unique identifier of the user." },
        new QueryArgument<StringGraphType>() { Name = "uniqueName", Description = "The unique name of the user." },
        new QueryArgument<CustomIdentifierInputGraphType>() { Name = "identifier", Description = "A custom identifier of the user." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IUserService, object?>().ReadAsync(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("uniqueName"),
        context.GetArgument<CustomIdentifierModel?>("identifier"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<UserSearchResultsGraphType>>("users")
      .Authorize()
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
