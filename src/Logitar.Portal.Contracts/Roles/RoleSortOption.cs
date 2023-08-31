namespace Logitar.Portal.Contracts.Roles;

public record RoleSortOption : SortOption
{
  public RoleSortOption() : this(RoleSort.UpdatedOn, isDescending: true)
  {
  }
  public RoleSortOption(RoleSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }

  public new RoleSort Field { get; set; }
}
