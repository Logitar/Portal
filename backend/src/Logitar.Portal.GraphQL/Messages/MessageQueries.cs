﻿using GraphQL;
using GraphQL.Types;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.GraphQL.Messages;

internal static class MessageQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<MessageGraphType>("message")
      .Authorize()
      .Description("Retrieves a single message.")
      .Arguments(
        new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "id", Description = "The unique identifier of the message." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IMessageService, object?>().ReadAsync(
        context.GetArgument<Guid>("id"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<MessageSearchResultsGraphType>>("messages")
      .Authorize()
      .Description("Searches a list of messages.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchMessagesPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IMessageService, object?>().SearchAsync(
        context.GetArgument<SearchMessagesPayload>("payload"),
        context.CancellationToken
      ));
  }
}
