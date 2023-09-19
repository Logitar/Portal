namespace Logitar.Portal.Contracts.Sessions;

public record SessionSortOption : SortOption
{
  public SessionSortOption() : this(SessionSort.UpdatedOn, isDescending: true)
  {
  }
  public SessionSortOption(SessionSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new SessionSort Field { get; set; }
}
