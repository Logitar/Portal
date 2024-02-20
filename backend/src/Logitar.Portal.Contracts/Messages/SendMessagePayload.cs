namespace Logitar.Portal.Contracts.Messages;

public record SendMessagePayload
{
  public Guid? SenderId { get; set; }
  public string Template { get; set; }

  public List<RecipientPayload> Recipients { get; set; }

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }

  public List<Variable> Variables { get; set; }

  public bool IsDemo { get; set; }

  public SendMessagePayload(string template)
  {
    Template = template;
    Recipients = [];
    Variables = [];
  }
}
