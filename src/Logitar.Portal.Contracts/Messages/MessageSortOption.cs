namespace Logitar.Portal.Contracts.Messages;

public record MessageSortOption : SortOption
{
  public MessageSortOption() : this(MessageSort.UpdatedOn, isDescending: true)
  {
  }
  public MessageSortOption(MessageSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new MessageSort Field { get; set; }
}
