using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.GraphQL.Templates;

internal class TemplateSearchResultsGraphType : SearchResultsGraphType<TemplateGraphType, Template>
{
  public TemplateSearchResultsGraphType() : base("TemplateSearchResults", "Represents the results of a template search.")
  {
  }
}
