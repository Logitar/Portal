using GraphQL;
using GraphQL.Types;
using Logitar.Cms.Schema.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.GraphQL.Roles;

namespace Logitar.Portal.GraphQL.Templates;

internal static class TemplateQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<RoleGraphType>("template")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Retrieves a single template.")
      .Arguments(
        new QueryArgument<IdGraphType>() { Name = "id", Description = "The unique identifier of the template." },
        new QueryArgument<StringGraphType>() { Name = "realm", Description = "The unique identifier or unique name of the realm in which the template resides." },
        new QueryArgument<StringGraphType>() { Name = "uniqueName", Description = "The unique name of the template." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IRoleService, object?>().ReadAsync(
        context.GetArgument<Guid?>("id"),
        context.GetArgument<string?>("realm"),
        context.GetArgument<string?>("uniqueName"),
        context.CancellationToken
      ));

    root.Field<NonNullGraphType<TemplateSearchResultsGraphType>>("templates")
      .AuthorizeWithPolicy(Policies.PortalActor)
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
