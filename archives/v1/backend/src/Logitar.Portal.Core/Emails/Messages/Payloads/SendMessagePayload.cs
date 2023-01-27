namespace Logitar.Portal.Core.Emails.Messages.Payloads
{
  public class SendMessagePayload
  {
    public string? Realm { get; set; }
    public string Template { get; set; } = null!;
    public Guid? SenderId { get; set; }

    public bool IgnoreUserLocale { get; set; }
    public string? Locale { get; set; }

    public IEnumerable<RecipientPayload> Recipients { get; set; } = null!;

    public IEnumerable<VariablePayload>? Variables { get; set; }
  }
}
