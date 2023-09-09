namespace Logitar.Portal.Contracts.Dictionaries;

public record DictionarySortOption : SortOption
{
  public DictionarySortOption() : this(DictionarySort.UpdatedOn, isDescending: true)
  {
  }
  public DictionarySortOption(DictionarySort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new DictionarySort Field { get; set; }
}
