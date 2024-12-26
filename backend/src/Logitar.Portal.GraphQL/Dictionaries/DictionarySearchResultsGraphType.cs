using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.GraphQL.Search;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal class DictionarySearchResultsGraphType : SearchResultsGraphType<DictionaryGraphType, DictionaryModel>
{
  public DictionarySearchResultsGraphType() : base("DictionarySearchResults", "Represents the results of a dictionary search.")
  {
  }
}
