using GraphQL;
using GraphQL.Types;
using Logitar.Cms.Schema.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.GraphQL.Sessions;

internal static class SessionQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<SessionGraphType>("session")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Retrieves a single session.")
      .Arguments(
        new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "id", Description = "The unique identifier of the session." }
      )
      .ResolveAsync(async context => await context.GetQueryService<ISessionService, object?>().ReadAsync(
        context.GetArgument<Guid>("id"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<SessionSearchResultsGraphType>>("sessions")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Searches a list of sessions.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchSessionsPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetQueryService<ISessionService, object?>().SearchAsync(
        context.GetArgument<SearchSessionsPayload>("payload"),
        context.CancellationToken
      ));
  }
}
