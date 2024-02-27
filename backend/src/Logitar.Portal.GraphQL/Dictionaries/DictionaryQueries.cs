using GraphQL;
using GraphQL.Types;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal static class DictionaryQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<DictionaryGraphType>("dictionary")
      .Authorize()
      .Description("Retrieves a single dictionary.")
      .Arguments(
        new QueryArgument<IdGraphType>() { Name = "id", Description = "The unique identifier of the dictionary." },
        new QueryArgument<StringGraphType>() { Name = "locale", Description = "The locale code of the dictionary." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IDictionaryService, object?>().ReadAsync(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("locale"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<DictionarySearchResultsGraphType>>("dictionaries")
      .Authorize()
      .Description("Searches a list of dictionaries.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchDictionariesPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IDictionaryService, object?>().SearchAsync(
        context.GetArgument<SearchDictionariesPayload>("payload"),
        context.CancellationToken
      ));
  }
}
