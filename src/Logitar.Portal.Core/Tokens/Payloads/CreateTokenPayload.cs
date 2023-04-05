namespace Logitar.Portal.Core.Tokens.Payloads
{
  public class CreateTokenPayload
  {
    public int? Lifetime { get; set; }
    public string? Purpose { get; set; }
    public string? Realm { get; set; }

    public string? Email { get; set; }
    public string? Subject { get; set; }

    public IEnumerable<ClaimPayload>? Claims { get; set; }
  }
}
