using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Models.Senders;

public record SearchSendersModel : SearchModel
{
  [FromQuery(Name = "provider")]
  public SenderProvider? Provider { get; set; }

  public SearchSendersPayload ToPayload()
  {
    SearchSendersPayload payload = new()
    {
      Provider = Provider
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new SenderSortOption(Enum.Parse<SenderSort>(sort)));
      }
      else
      {
        SenderSort field = Enum.Parse<SenderSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new SenderSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
