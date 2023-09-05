using GraphQL.Types;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.GraphQL;

internal class TextSearchInputGraphType : InputObjectGraphType<TextSearch>
{
  public TextSearchInputGraphType()
  {
    Name = nameof(TextSearch);
    Description = "Represents textual search parameters.";

    Field(x => x.Terms, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<SearchTermInputGraphType>>>))
      .Description("The terms of the textual search.");
    Field(x => x.Operator, type: typeof(NonNullGraphType<SearchOperatorGraphType>))
      .Description("The operator of the textual search.");
  }
}
