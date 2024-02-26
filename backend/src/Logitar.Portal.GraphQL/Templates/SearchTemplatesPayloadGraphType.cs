using GraphQL.Types;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Templates;

internal class SearchTemplatesPayloadGraphType : SearchPayloadInputGraphType<SearchTemplatesPayload>
{
  public SearchTemplatesPayloadGraphType() : base()
  {
    Field(x => x.ContentType, nullable: true)
      .Description("When specified, will filter out templates that do not have the specified content type.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<TemplateSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
