using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Models.Search;

namespace Logitar.Portal.Models.Realms;

public record SearchRealmsModel : SearchModel
{
  public SearchRealmsPayload ToPayload()
  {
    SearchRealmsPayload payload = new();
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new RealmSortOption(Enum.Parse<RealmSort>(sort)));
      }
      else
      {
        RealmSort field = Enum.Parse<RealmSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new RealmSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
