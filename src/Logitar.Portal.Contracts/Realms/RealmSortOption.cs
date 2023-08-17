namespace Logitar.Portal.Contracts.Realms;

public record RealmSortOption : SortOption
{
  public RealmSortOption() : this(RealmSort.UpdatedOn, isDescending: true)
  {
  }
  public RealmSortOption(RealmSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new RealmSort Field { get; set; }
}
