using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Dictionaries;

public record SearchDictionariesPayload : SearchPayload
{
  public new List<DictionarySortOption> Sort { get; set; } = [];
}
