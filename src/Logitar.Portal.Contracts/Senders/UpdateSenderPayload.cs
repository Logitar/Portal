namespace Logitar.Portal.Contracts.Senders;

public record UpdateSenderPayload
{
  public string? EmailAddress { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public IEnumerable<ProviderSettingModification> Settings { get; set; } = Enumerable.Empty<ProviderSettingModification>();
}
