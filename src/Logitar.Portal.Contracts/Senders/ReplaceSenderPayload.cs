namespace Logitar.Portal.Contracts.Senders;

public record ReplaceSenderPayload
{
  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public IEnumerable<ProviderSetting> Settings { get; set; } = Enumerable.Empty<ProviderSetting>();
}
