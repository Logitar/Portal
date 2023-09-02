namespace Logitar.Portal.Contracts.Tokens;

public record CreateTokenPayload
{
  public bool IsConsumable { get; set; }
  public string? Purpose { get; set; }
  public string? Realm { get; set; }

  public string? Algorithm { get; set; }
  public string? Audience { get; set; }
  public string? Issuer { get; set; }
  public int Lifetime { get; set; }
  public string? Secret { get; set; }

  public string? Subject { get; set; }
  public string? EmailAddress { get; set; }
  public IEnumerable<Claim> Claims { get; set; } = Enumerable.Empty<Claim>();
}
