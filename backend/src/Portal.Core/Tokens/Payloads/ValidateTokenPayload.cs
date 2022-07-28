namespace Portal.Core.Tokens.Payloads
{
  public class ValidateTokenPayload
  {
    public string Token { get; set; } = null!;

    public string? Purpose { get; set; }
    public string? Realm { get; set; }
  }
}
