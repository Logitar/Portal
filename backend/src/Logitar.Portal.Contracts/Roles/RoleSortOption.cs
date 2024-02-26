using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Roles;

public record RoleSortOption : SortOption
{
  public new RoleSort Field
  {
    get => Enum.Parse<RoleSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public RoleSortOption() : this(RoleSort.UpdatedOn, isDescending: true)
  {
  }

  public RoleSortOption(RoleSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
