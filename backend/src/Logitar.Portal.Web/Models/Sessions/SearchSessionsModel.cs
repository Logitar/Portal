﻿using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Models.Sessions;

public record SearchSessionsModel : SearchModel
{
  [FromQuery(Name = "user_id")]
  public Guid? UserId { get; set; }

  [FromQuery(Name = "active")]
  public bool? IsActive { get; set; }

  [FromQuery(Name = "persistent")]
  public bool? IsPersistent { get; set; }

  public SearchSessionsPayload ToPayload()
  {
    SearchSessionsPayload payload = new()
    {
      UserId = UserId,
      IsActive = IsActive,
      IsPersistent = IsPersistent
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new SessionSortOption(Enum.Parse<SessionSort>(sort)));
      }
      else
      {
        SessionSort field = Enum.Parse<SessionSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new SessionSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
