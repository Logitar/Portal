using GraphQL.Types;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal class SearchDictionariesPayloadGraphType : SearchPayloadInputGraphType<SearchDictionariesPayload>
{
  public SearchDictionariesPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<DictionarySortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
