using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Models.Search;

namespace Logitar.Portal.Models.Roles;

public record SearchRolesModel : SearchModel
{
  public SearchRolesPayload ToPayload()
  {
    SearchRolesPayload payload = new();
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new RoleSortOption(Enum.Parse<RoleSort>(sort)));
      }
      else
      {
        RoleSort field = Enum.Parse<RoleSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new RoleSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
