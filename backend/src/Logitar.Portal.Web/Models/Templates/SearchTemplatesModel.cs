using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Models.Templates;

public record SearchTemplatesModel : SearchModel
{
  [FromQuery(Name = "type")]
  public string? ContentType { get; set; }

  public SearchTemplatesPayload ToPayload()
  {
    SearchTemplatesPayload payload = new()
    {
      ContentType = ContentType
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new TemplateSortOption(Enum.Parse<TemplateSort>(sort)));
      }
      else
      {
        TemplateSort field = Enum.Parse<TemplateSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new TemplateSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
