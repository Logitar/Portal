namespace Logitar.Portal.Contracts.ApiKeys;

public record SearchApiKeysPayload : SearchPayload
{
  public string? Realm { get; set; }

  public ApiKeyStatus? Status { get; set; }

  public new IEnumerable<ApiKeySortOption> Sort { get; set; } = Enumerable.Empty<ApiKeySortOption>();
}
