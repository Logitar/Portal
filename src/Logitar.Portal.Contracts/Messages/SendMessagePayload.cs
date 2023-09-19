namespace Logitar.Portal.Contracts.Messages;

public record SendMessagePayload
{
  public string? Realm { get; set; }
  public Guid? SenderId { get; set; }
  public string Template { get; set; } = string.Empty;

  public IEnumerable<RecipientPayload> Recipients { get; set; } = Enumerable.Empty<RecipientPayload>();

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }

  public IEnumerable<Variable> Variables { get; set; } = Enumerable.Empty<Variable>();
}
