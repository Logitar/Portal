using GraphQL;
using GraphQL.Types;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.GraphQL.Roles;

namespace Logitar.Portal.GraphQL.Templates;

internal static class TemplateQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<RoleGraphType>("template")
      .Authorize()
      .Description("Retrieves a single template.")
      .Arguments(
        new QueryArgument<IdGraphType>() { Name = "id", Description = "The unique identifier of the template." },
        new QueryArgument<StringGraphType>() { Name = "uniqueKey", Description = "The unique key of the template." }
      )
      .ResolveAsync(async context => await context.GetQueryService<ITemplateService, object?>().ReadAsync(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("uniqueKey"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<TemplateSearchResultsGraphType>>("templates")
      .Authorize()
      .Description("Searches a list of templates.")
      .Arguments(
        new QueryArgument<NonNullGraphType<SearchTemplatesPayloadGraphType>>() { Name = "payload", Description = "The parameters to apply to the search." }
      )
      .ResolveAsync(async context => await context.GetQueryService<ITemplateService, object?>().SearchAsync(
        context.GetArgument<SearchTemplatesPayload>("payload"),
        context.CancellationToken
      ));
  }
}
