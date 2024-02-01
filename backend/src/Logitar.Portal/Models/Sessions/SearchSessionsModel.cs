using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Models.Sessions;

public record SearchSessionsModel : SearchModel
{
  [FromQuery(Name = "user_id")]
  public Guid? UserId { get; set; }

  [FromQuery(Name = "active")]
  public bool? IsActive { get; set; }

  [FromQuery(Name = "persistent")]
  public bool? IsPersistent { get; set; }

  public SearchSessionsPayload ToPayload() // TODO(fpion): refactor
  {
    SearchSessionsPayload payload = new()
    {
      Ids = Ids,
      Search = new TextSearch(SearchTerms.Select(value => new SearchTerm(value)), SearchOperator),
      UserId = UserId,
      IsActive = IsActive,
      IsPersistent = IsPersistent,
      Skip = Skip,
      Limit = Limit
    };

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
