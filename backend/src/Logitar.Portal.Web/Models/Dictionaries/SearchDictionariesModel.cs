﻿using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Models.Dictionaries;

public record SearchDictionariesModel : SearchModel
{
  [FromQuery(Name = "empty")]
  public bool? IsEmpty { get; set; }

  public SearchDictionariesPayload ToPayload()
  {
    SearchDictionariesPayload payload = new()
    {
      IsEmpty = IsEmpty
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new DictionarySortOption(Enum.Parse<DictionarySort>(sort)));
      }
      else
      {
        DictionarySort field = Enum.Parse<DictionarySort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new DictionarySortOption(field, isDescending));
      }
    }

    return payload;
  }
}
