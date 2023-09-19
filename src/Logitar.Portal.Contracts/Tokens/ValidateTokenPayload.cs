namespace Logitar.Portal.Contracts.Tokens;

public record ValidateTokenPayload
{
  public ValidateTokenPayload() : this(string.Empty)
  {
  }
  public ValidateTokenPayload(string token)
  {
    Token = token;
  }

  public string Token { get; set; }

  public bool Consume { get; set; }
  public string? Realm { get; set; }

  public string? Audience { get; set; }
  public string? Issuer { get; set; }
  public string? Secret { get; set; }
  public string? Type { get; set; }
}
