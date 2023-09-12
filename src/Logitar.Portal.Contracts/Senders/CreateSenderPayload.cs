namespace Logitar.Portal.Contracts.Senders;

public record CreateSenderPayload
{
  public string? Realm { get; set; }

  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ProviderType Provider { get; set; }
  public IEnumerable<ProviderSetting> Settings { get; set; } = Enumerable.Empty<ProviderSetting>();
}
