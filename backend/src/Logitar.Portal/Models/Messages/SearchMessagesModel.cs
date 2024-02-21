using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Models.Messages;

public record SearchMessagesModel : SearchModel
{
  [FromQuery(Name = "template_id")]
  public Guid? TemplateId { get; set; }

  [FromQuery(Name = "demo")]
  public bool? IsDemo { get; set; }

  [FromQuery(Name = "status")]
  public MessageStatus? Status { get; set; }

  public SearchMessagesPayload ToPayload()
  {
    SearchMessagesPayload payload = new()
    {
      TemplateId = TemplateId,
      IsDemo = IsDemo,
      Status = Status
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new MessageSortOption(Enum.Parse<MessageSort>(sort)));
      }
      else
      {
        MessageSort field = Enum.Parse<MessageSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new MessageSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
