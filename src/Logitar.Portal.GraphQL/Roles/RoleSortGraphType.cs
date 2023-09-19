using GraphQL.Types;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.GraphQL.Roles;

internal class RoleSortGraphType : EnumerationGraphType<RoleSort>
{
  public RoleSortGraphType()
  {
    Name = nameof(RoleSort);
    Description = "Represents the available role fields for sorting.";

    Add(RoleSort.DisplayName, "The roles will be sorted by their display name.");
    Add(RoleSort.UniqueName, "The roles will be sorted by their unique name.");
    Add(RoleSort.UpdatedOn, "The roles will be sorted by their latest update date and time.");
  }

  private void Add(RoleSort value, string description) => Add(value.ToString(), value, description);
}
