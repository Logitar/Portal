using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Models.Users;

public record SearchUsersModel : SearchModel
{
  [FromQuery(Name = "has_authenticated")]
  public bool? HasAuthenticated { get; set; }

  [FromQuery(Name = "has_password")]
  public bool? HasPassword { get; set; }

  [FromQuery(Name = "disabled")]
  public bool? IsDisabled { get; set; }

  [FromQuery(Name = "confirmed")]
  public bool? isConfirmed { get; set; }

  public SearchUsersPayload ToPayload()
  {
    SearchUsersPayload payload = new()
    {
      HasAuthenticated = HasAuthenticated,
      HasPassword = HasPassword,
      IsDisabled = IsDisabled,
      IsConfirmed = isConfirmed
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new UserSortOption(Enum.Parse<UserSort>(sort)));
      }
      else
      {
        UserSort field = Enum.Parse<UserSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new UserSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
