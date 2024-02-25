using GraphQL.Types;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.GraphQL.Search;

internal class SearchTermGraphType : InputObjectGraphType<SearchTerm>
{
  public SearchTermGraphType()
  {
    Name = nameof(SearchTerm);
    Description = "Represents a search term.";

    Field(x => x.Value)
      .Description("The textual value of the search term.");
  }
}
