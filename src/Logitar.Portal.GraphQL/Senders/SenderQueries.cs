using GraphQL;
using GraphQL.Types;
using Logitar.Cms.Schema.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal static class SenderQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<SenderGraphType>("defaultSender")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Retrieves a default sender.")
      .Arguments(
        new QueryArgument<StringGraphType>() { Name = "realm", Description = "The unique identifier or unique name of the realm in which the sender resides." }
      )
      .ResolveAsync(async context => await context.GetQueryService<ISenderService, object?>().ReadDefaultAsync(
        context.GetArgument<string?>("realm"),
        context.CancellationToken
      ));

    root.Field<SenderGraphType>("sender")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Retrieves a single sender.")
      .Arguments(
        new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "id", Description = "The unique identifier of the sender." }
      )
      .ResolveAsync(async context => await context.GetQueryService<ISenderService, object?>().ReadAsync(
        context.GetArgument<Guid>("id"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<SenderSearchResultsGraphType>>("senders")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Searches a list of senders.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchSendersPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetQueryService<ISenderService, object?>().SearchAsync(
        context.GetArgument<SearchSendersPayload>("payload"),
        context.CancellationToken
      ));
  }
}
