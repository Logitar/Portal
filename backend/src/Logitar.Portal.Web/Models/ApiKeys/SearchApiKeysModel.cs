using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Models.ApiKeys;

public record SearchApiKeysModel : SearchModel
{
  [FromQuery(Name = "expired")]
  public bool? IsExpired { get; set; }

  [FromQuery(Name = "moment")]
  public DateTime? Moment { get; set; }

  public SearchApiKeysPayload ToPayload()
  {
    SearchApiKeysPayload payload = new();
    if (IsExpired.HasValue)
    {
      payload.Status = new ApiKeyStatus(IsExpired.Value, Moment);
    }
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new ApiKeySortOption(Enum.Parse<ApiKeySort>(sort)));
      }
      else
      {
        ApiKeySort field = Enum.Parse<ApiKeySort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new ApiKeySortOption(field, isDescending));
      }
    }

    return payload;
  }
}
