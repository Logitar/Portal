namespace Logitar.Portal.Contracts.Messages
{
  public record SendMessagePayload
  {
    public string? Realm { get; set; }
    public string Template { get; set; } = string.Empty;
    public string? SenderId { get; set; }

    public bool IgnoreUserLocale { get; set; }
    public string? Locale { get; set; }

    public IEnumerable<RecipientPayload> Recipients { get; set; } = Enumerable.Empty<RecipientPayload>();

    public IEnumerable<VariablePayload>? Variables { get; set; }
  }
}
