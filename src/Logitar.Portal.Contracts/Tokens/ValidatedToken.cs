namespace Logitar.Portal.Contracts.Tokens;

public record ValidatedToken
{
  public string? Subject { get; set; }
  public string? EmailAddress { get; set; }
  public IEnumerable<Claim> Claims { get; set; } = Enumerable.Empty<Claim>();
}
