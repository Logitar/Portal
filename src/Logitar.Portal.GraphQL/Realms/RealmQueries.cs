using GraphQL;
using GraphQL.Types;
using Logitar.Cms.Schema.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.GraphQL.Realms;

internal static class RealmQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<RealmGraphType>("realm")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Retrieves a single realm.")
      .Arguments(
        new QueryArgument<IdGraphType>() { Name = "id", Description = "The unique identifier of the realm." },
        new QueryArgument<StringGraphType>() { Name = "uniqueSlug", Description = "The unique slug of the realm." }
      )
      .ResolveAsync(async context => await context.GetRequiredService<IRealmService, object?>().ReadAsync(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("uniqueSlug"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<RealmSearchResultsGraphType>>("realms")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Searches a list of realms.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchRealmsPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetRequiredService<IRealmService, object?>().SearchAsync(
        context.GetArgument<SearchRealmsPayload>("payload"),
        context.CancellationToken
      ));
  }
}
