namespace Logitar.Portal.Contracts.Users;

public record UserSortOption : SortOption
{
  public UserSortOption() : this(UserSort.UpdatedOn, isDescending: true)
  {
  }
  public UserSortOption(UserSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new UserSort Field { get; set; }
}
