namespace Logitar.Portal.Contracts.Senders;

public record SenderSortOption : SortOption
{
  public SenderSortOption() : this(SenderSort.UpdatedOn, isDescending: true)
  {
  }
  public SenderSortOption(SenderSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new SenderSort Field { get; set; }
}
