using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Models.Search;

namespace Logitar.Portal.Models.Roles;

public record SearchRolesModel : SearchModel
{
  public SearchRolesPayload ToPayload() // TODO(fpion): refactor
  {
    SearchRolesPayload payload = new()
    {
      Ids = Ids,
      Search = new TextSearch(SearchTerms.Select(value => new SearchTerm(value)), SearchOperator),
      Skip = Skip,
      Limit = Limit
    };

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
