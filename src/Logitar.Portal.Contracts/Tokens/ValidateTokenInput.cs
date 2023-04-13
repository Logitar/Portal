namespace Logitar.Portal.Contracts.Tokens;

public record ValidateTokenInput
{
  public string Token { get; set; } = string.Empty;

  public string? Purpose { get; set; }
  public string? Realm { get; set; }
  public string? Secret { get; set; }

  public string? Audience { get; set; }
  public string? Issuer { get; set; }
}
