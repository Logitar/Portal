namespace Logitar.Portal.Contracts.Tokens
{
  public record ValidateTokenPayload
  {
    public string Token { get; set; } = string.Empty;

    public string? Purpose { get; set; }
    public string? Realm { get; set; }
  }
}
