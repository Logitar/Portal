﻿using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Templates;

internal class TemplateSearchResultsGraphType : SearchResultsGraphType<TemplateGraphType, TemplateModel>
{
  public TemplateSearchResultsGraphType() : base("TemplateSearchResults", "Represents the results of a template search.")
  {
  }
}
