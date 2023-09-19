using GraphQL.Types;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal class SearchDictionariesPayloadGraphType : SearchPayloadInputGraphType<SearchDictionariesPayload>
{
  public SearchDictionariesPayloadGraphType() : base()
  {
    Field(x => x.Realm, nullable: true)
      .Description("The unique identifier or unique name of the realm in which to search.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<DictionarySortOptionGraphType>>>))
      .DefaultValue(Enumerable.Empty<DictionarySortOption>())
      .Description("The sort parameters of the search.");
  }
}
