namespace Logitar.Portal.Contracts.Messages;

public record SendMessageInput
{
  public string Realm { get; set; } = string.Empty;
  public string Template { get; set; } = string.Empty;
  public Guid? SenderId { get; set; }

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }

  public IEnumerable<RecipientInput> Recipients { get; set; } = Enumerable.Empty<RecipientInput>();

  public IEnumerable<Variable>? Variables { get; set; }
}
