using GraphQL.Types;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal class DictionarySortGraphType : EnumerationGraphType<DictionarySort>
{
  public DictionarySortGraphType()
  {
    Name = nameof(DictionarySort);
    Description = "Represents the available dictionary fields for sorting.";

    Add(DictionarySort.EntryCount, "The dictionaries will be sorted by their number of entries.");
    Add(DictionarySort.Locale, "The dictionaries will be sorted by their locale code.");
    Add(DictionarySort.UpdatedOn, "The dictionaries will be sorted by their latest update date and time.");
  }

  private void Add(DictionarySort value, string description) => Add(value.ToString(), value, description);
}
