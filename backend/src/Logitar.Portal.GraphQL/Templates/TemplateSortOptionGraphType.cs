using GraphQL.Types;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Templates;

internal class TemplateSortOptionGraphType : SortOptionInputGraphType<TemplateSortOption>
{
  public TemplateSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<TemplateSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
