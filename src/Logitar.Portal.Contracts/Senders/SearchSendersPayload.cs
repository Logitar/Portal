namespace Logitar.Portal.Contracts.Senders;

public record SearchSendersPayload : SearchPayload
{
  public string? Realm { get; set; }

  public ProviderType? Provider { get; set; }

  public new IEnumerable<SenderSortOption> Sort { get; set; } = Enumerable.Empty<SenderSortOption>();
}
