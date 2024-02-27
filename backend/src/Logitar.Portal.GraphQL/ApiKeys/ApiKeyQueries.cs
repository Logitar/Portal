using GraphQL;
using GraphQL.Types;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.GraphQL.ApiKeys;

internal static class ApiKeyQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<ApiKeyGraphType>("apiKey")
      .Authorize()
      .Description("Retrieves a single API key.")
      .Arguments(
        new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "id", Description = "The unique identifier of the API key." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IApiKeyService, object?>().ReadAsync(
        context.GetArgument<Guid>("id"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<ApiKeySearchResultsGraphType>>("apiKeys")
      .Authorize()
      .Description("Searches a list of API keys.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchApiKeysPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IApiKeyService, object?>().SearchAsync(
        context.GetArgument<SearchApiKeysPayload>("payload"),
        context.CancellationToken
      ));
  }
}
