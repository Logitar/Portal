using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Contracts.Senders;

public record Sender : Aggregate
{
  public Guid Id { get; set; }

  public bool IsDefault { get; set; }

  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public ProviderType Provider { get; set; }
  public IEnumerable<Setting> Settings { get; set; } = Enumerable.Empty<Setting>();

  public Realm Realm { get; set; } = new();
}
