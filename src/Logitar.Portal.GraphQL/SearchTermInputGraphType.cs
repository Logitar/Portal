using GraphQL.Types;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.GraphQL;

internal class SearchTermInputGraphType : InputObjectGraphType<SearchTerm>
{
  public SearchTermInputGraphType()
  {
    Name = nameof(SearchTerm);
    Description = "Represents a search term.";

    Field(x => x.Value)
      .Description("The textual value of the search term.");
  }
}
