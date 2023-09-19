using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Senders;

public record Sender : Aggregate
{
  public Guid Id { get; set; }

  public bool IsDefault { get; set; }

  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ProviderType Provider { get; set; }
  public IEnumerable<ProviderSetting> Settings { get; set; } = Enumerable.Empty<ProviderSetting>();

  public Realm? Realm { get; set; }
}
