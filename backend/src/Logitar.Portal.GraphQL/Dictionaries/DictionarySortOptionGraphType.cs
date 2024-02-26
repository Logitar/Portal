using GraphQL.Types;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal class DictionarySortOptionGraphType : SortOptionInputGraphType<DictionarySortOption>
{
  public DictionarySortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<DictionarySortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
